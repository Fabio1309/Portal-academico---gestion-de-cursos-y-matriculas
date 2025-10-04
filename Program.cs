using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// Asegúrate de que el namespace coincida con el de tu proyecto
using examen_parcial_programacion1.Data; 
using examen_parcial_programacion1.Models;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICIOS ---

// 1. Configuración de la Base de Datos (DbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Configuración de Identity con soporte para Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // <-- AÑADIDO: Habilita la gestión de roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Configuración de Redis para Caché Distribuida
var redisConnectionString = builder.Configuration["Redis_ConnectionString"];
if (!string.IsNullOrEmpty(redisConnectionString))
{
    // Si encuentra la connection string, usa Redis
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "PortalAcademico_"; // Prefijo para las claves de caché
    });
}
else
{
    // Si no, usa caché en memoria (ideal para desarrollo local sin Redis)
    builder.Services.AddDistributedMemoryCache();
}

// 4. Configuración de Sesiones (usa la caché configurada arriba)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Tiempo de inactividad antes de que la sesión expire
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 5. Configuración de Controladores y Vistas
builder.Services.AddControllersWithViews();

// --- CONSTRUCCIÓN DE LA APP ---

var app = builder.Build();

// --- LÓGICA DE INICIO (SEEDING) ---
// Se ejecuta una vez al iniciar la aplicación para poblar la base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Aplica cualquier migración pendiente para asegurar que la BD está actualizada
        context.Database.Migrate();

        // Crear el rol "Coordinador" si no existe
        if (!await roleManager.RoleExistsAsync("Coordinador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));
        }

        // Crear el usuario coordinador si no existe
        var coordinadorEmail = "coordinador@test.com";
        if (await userManager.FindByEmailAsync(coordinadorEmail) == null)
        {
            var coordinadorUser = new IdentityUser { UserName = coordinadorEmail, Email = coordinadorEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(coordinadorUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(coordinadorUser, "Coordinador");
            }
        }

        // Crear cursos de ejemplo si la tabla está vacía
        if (!context.Cursos.Any())
        {
            context.Cursos.AddRange(
                new Curso { Codigo = "CS101", Nombre = "Introducción a la Programación", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeOnly(8, 0), HorarioFin = new TimeOnly(10, 0), Activo = true },
                new Curso { Codigo = "MA201", Nombre = "Cálculo Avanzado", Creditos = 5, CupoMaximo = 25, HorarioInicio = new TimeOnly(10, 0), HorarioFin = new TimeOnly(12, 0), Activo = true },
                new Curso { Codigo = "DB301", Nombre = "Bases de Datos", Creditos = 4, CupoMaximo = 20, HorarioInicio = new TimeOnly(14, 0), HorarioFin = new TimeOnly(16, 0), Activo = true },
                new Curso { Codigo = "XX000", Nombre = "Curso Inactivo", Creditos = 3, CupoMaximo = 10, HorarioInicio = new TimeOnly(9, 0), HorarioFin = new TimeOnly(11, 0), Activo = false }
            );
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante el proceso de seeding de la base de datos.");
    }
}


// --- PIPELINE DE MIDDLEWARE HTTP ---

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// <-- AÑADIDO: Habilita el middleware de sesión.
// ¡El orden es importante! Debe ir después de UseRouting y antes de UseAuthorization.
app.UseSession(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();