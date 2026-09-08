using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using FinalProgramacion4.GymRutinas.api.Data;
using Microsoft.EntityFrameworkCore;
using FinalProgramacion4.GymRutinas.api.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AplicationDbContext>(options => options.UseSqlServer(connectionString));



builder.Services.AddScoped<ServiceEjercicio>();
builder.Services.AddScoped<ServiceEjercicio>();
builder.Services.AddControllers();

var app = builder.Build();

using (var aux = app.Services.CreateScope())
{
    var dbContext = aux.ServiceProvider.GetRequiredService<AplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
