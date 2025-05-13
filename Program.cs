using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using VN_Center.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<VNCenterDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de Identity
builder.Services.AddIdentity<UsuariosSistema, RolesSistema>(options => {
  // Configuraciones de Identity (ej. requisitos de contraseña)
  options.SignIn.RequireConfirmedAccount = false; // Cambia a true si quieres confirmación de email
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 8;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = true;
  options.Password.RequireLowercase = true;
  options.Password.RequiredUniqueChars = 1;

  // Configuraciones de Lockout
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;

  // Configuraciones de Usuario
  options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
  options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<VNCenterDbContext>()
    .AddDefaultTokenProviders(); // Necesario para reseteo de contraseña, etc.
                                 //.AddDefaultUI(); // Si usas la UI por defecto de Identity (Razor Pages)

builder.Services.AddControllersWithViews();

// Si no usas la UI por defecto de Identity y tienes tu propio AuthController,
// podrías necesitar configurar las rutas para login/logout aquí o en UseEndpoints.
// builder.Services.AddRazorPages(); // Si usas Razor Pages para Identity UI

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint(); // Útil para desarrollo con migraciones
                               // Llamar al DataSeeder aquí
  try
  {
    // Pasa IConfiguration al seeder
    await DataSeeder.Initialize(app.Services, app.Configuration);
    Console.WriteLine("Data seeding completed or verified.");
  }
  catch (Exception ex)
  {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB.");
    Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
  }
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Asegúrate que esto está ANTES de UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages(); // Si usas Razor Pages para Identity UI

app.Run();
