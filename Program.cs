using EMPLOYEE_MANAGER.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Agrega soporte para acceder a HttpContext (para leer cookies)
builder.Services.AddHttpContextAccessor();

// ⏬ La conexión ya no se decide aquí, se configurará dinámicamente en AppDbContext
builder.Services.AddDbContext<AppDbContext>();

// Agrega servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
