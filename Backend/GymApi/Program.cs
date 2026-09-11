// ------------------------------------------------------------
// Program.cs: punto de entrada de la API
// ------------------------------------------------------------
// Tiene dos partes bien separadas:
//   1) builder.Services...  -> REGISTRAR servicios en el contenedor de
//      inyección de dependencias (qué clases existen y cómo se crean).
//   2) app.Use.../app.Map... -> armar el PIPELINE: por qué pasos atraviesa
//      cada request HTTP antes de llegar a un controller.
// (En la Etapa 2 se completa con CORS, Swagger y manejo de errores.)

using GymApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== 1) REGISTRO DE SERVICIOS =====

// Habilita los Controllers (clases con [ApiController] que atienden rutas).
builder.Services.AddControllers();

// Genera el documento OpenAPI (descripción JSON de todos los endpoints).
builder.Services.AddOpenApi();

// Registra el DbContext para SQL Server.
// GetConnectionString("DefaultConnection") lee la sección
// "ConnectionStrings" de appsettings.json.
// AddDbContext lo registra como "Scoped": una instancia NUEVA por cada
// request HTTP, que se descarta al terminar la request.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GymDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// ===== 2) PIPELINE HTTP =====

if (app.Environment.IsDevelopment())
{
    // Publica el documento OpenAPI en /openapi/v1.json (solo en desarrollo).
    app.MapOpenApi();
}

// Conecta las rutas definidas con [Route] en los controllers.
app.MapControllers();

app.Run();
