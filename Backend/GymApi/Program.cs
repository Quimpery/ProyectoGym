// ------------------------------------------------------------
// Program.cs: punto de entrada de la API
// ------------------------------------------------------------
// Tiene dos partes bien separadas:
//   1) builder.Services...  -> REGISTRAR servicios en el contenedor de
//      inyección de dependencias (qué clases existen y cómo se crean).
//   2) app.Use.../app.Map... -> armar el PIPELINE: por qué pasos atraviesa
//      cada request HTTP antes de llegar a un controller. EL ORDEN IMPORTA.

using System.Text.Json.Serialization;
using GymApi.Data;
using GymApi.Dtos;
using GymApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===================== 1) REGISTRO DE SERVICIOS =====================

builder.Services
    // Habilita los Controllers (clases con [ApiController]).
    .AddControllers(options =>
    {
        // Sin esto, cuando el JSON no se puede leer, ASP.NET agrega además un
        // error automático en inglés ("The dto field is required.").
        // Nuestros DTOs ya marcan con [Required] lo que es obligatorio.
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    // Configuración del JSON:
    // JsonStringEnumConverter hace que los enums viajen como TEXTO
    // ("Lunes") en vez de número (1). allowIntegerValues: false rechaza
    // números, así un "dia": 9 da error en vez de guardarse.
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    })
    // Cambiamos el formato del 400 automático de [ApiController] para que use
    // nuestro ErrorRespuesta { mensaje, errores } igual que el resto de errores.
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // ModelState tiene, por cada campo, la lista de errores encontrados.
            // Juntamos todos los mensajes en una sola lista de textos.
            List<string> errores = context.ModelState
                .Where(campo => campo.Value!.Errors.Count > 0)
                .SelectMany(campo => campo.Value!.Errors.Select(error => TraducirError(campo.Key, error.ErrorMessage)))
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(new ErrorRespuesta("Los datos enviados no son válidos.", errores));
        };
    });

// Cuando el JSON ni siquiera se puede convertir al DTO, ASP.NET no usa
// nuestros mensajes sino uno técnico en inglés, y la clave del campo empieza
// con "$" (ej. "$.ejercicios[0].dia"). Acá lo reemplazamos por uno entendible.
static string TraducirError(string campo, string mensajeOriginal)
{
    // "$" o "" = el problema es el JSON completo (mal escrito o vacío).
    if (campo == "$" || campo == "")
    {
        return "El cuerpo de la petición no es un JSON válido.";
    }
    // "$.series", "$.ejercicios[0].dia" = un valor con tipo incorrecto
    // (ej. "series": "abc" o "dia": "Lunez").
    if (campo.StartsWith("$."))
    {
        return $"El campo '{campo.Substring(2)}' tiene un valor inválido.";
    }
    // Resto de los casos: son nuestros mensajes de los atributos [Required], [Range]...
    return mensajeOriginal;
}

// Genera el documento OpenAPI (descripción JSON de todos los endpoints),
// que después muestra Swagger UI.
builder.Services.AddOpenApi();

// El generador de OpenAPI lee OTRA configuración de JSON (la de "HttpJson").
// Le agregamos el mismo conversor para que Swagger muestre los días como
// texto ("Lunes", "Martes"...) y no como números.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
});

// Registra el DbContext para SQL Server.
// GetConnectionString("DefaultConnection") lee la sección
// "ConnectionStrings" de appsettings.json.
// AddDbContext lo registra como "Scoped": una instancia NUEVA por cada
// request HTTP, que se descarta al terminar la request.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlServer(connectionString));

// Registra nuestro service. También Scoped, porque usa el DbContext (que es Scoped).
// Gracias a esto, el constructor de RutinasController puede pedir un RutinaService.
builder.Services.AddScoped<RutinaService>();

// CORS (Cross-Origin Resource Sharing):
// Angular corre en http://localhost:4200 y la API en http://localhost:5000.
// Para el navegador son "orígenes" distintos y BLOQUEA la respuesta salvo que
// la API diga explícitamente que ese origen está permitido.
const string PoliticaCors = "PermitirAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCors, policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()     // permite headers como Content-Type
              .AllowAnyMethod());   // permite GET, POST, PUT, DELETE
});

var app = builder.Build();

// ===================== CREAR / ACTUALIZAR LA BASE =====================
// Al arrancar, aplica las migraciones pendientes (equivale a correr
// "dotnet ef database update"). Si la base GymRutinasDb no existe, la crea.
// Usamos un "scope" porque el DbContext es Scoped y acá no hay request HTTP.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    dbContext.Database.Migrate();
}

// ===================== 2) PIPELINE HTTP =====================

// Manejo global de errores: si en cualquier parte se lanza una excepción no
// controlada, respondemos 500 con nuestro formato en vez de un error feo.
// (El detalle de la excepción queda registrado en la consola de la API.)
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(
            new ErrorRespuesta("Ocurrió un error inesperado en el servidor. Intentá de nuevo más tarde."));
    });
});

if (app.Environment.IsDevelopment())
{
    // Publica el documento OpenAPI en /openapi/v1.json
    app.MapOpenApi();
    // Swagger UI: página web para ver y PROBAR los endpoints en /swagger
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "GymApi v1"));
}

// Aplica la política CORS. Tiene que ir ANTES de MapControllers.
app.UseCors(PoliticaCors);

// Conecta las rutas definidas con [Route]/[HttpGet]... en los controllers.
app.MapControllers();

app.Run();
