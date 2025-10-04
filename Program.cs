using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using examen_parcial_programacion1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Crear rol "Coordinador" si no existe
        if (!await roleManager.RoleExistsAsync("Coordinador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));
        }

        // Crear usuario coordinador si no existe
        var coordinadorEmail = "coordinador@test.com";
        if (await userManager.FindByEmailAsync(coordinadorEmail) == null)
        {
            var coordinador = new IdentityUser { UserName = coordinadorEmail, Email = coordinadorEmail, EmailConfirmed = true };
            await userManager.CreateAsync(coordinador, "Password123!");
            await userManager.AddToRoleAsync(coordinador, "Coordinador");
        }

        // Crear cursos si no existen
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
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
