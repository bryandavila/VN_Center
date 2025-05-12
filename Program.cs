using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities; // Asegúrate que este using esté presente
using Microsoft.AspNetCore.Identity; // Necesario para AddIdentity y AddEntityFrameworkStores

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración del DbContext (ya lo teníamos)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VNCenterDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configuración de ASP.NET Core Identity
// Aquí le decimos a Identity que use VNCenterDbContext para almacenar sus datos
// y que nuestras clases personalizadas son UsuariosSistema y RolesSistema.
// Usamos AddDefaultTokenProviders() para funcionalidades como la generación de tokens para reseteo de contraseña.
builder.Services.AddIdentity<UsuariosSistema, RolesSistema>(options => {
  // Configuración de opciones de Identity (puedes ajustar esto según tus necesidades)
  // Opciones de Contraseña
  options.Password.RequireDigit = true; // Requerir al menos un dígito
  options.Password.RequireLowercase = true; // Requerir al menos una minúscula
  options.Password.RequireUppercase = true; // Requerir al menos una mayúscula
  options.Password.RequireNonAlphanumeric = true; // Requerir al menos un caracter no alfanumérico (ej. @, #, !)
  options.Password.RequiredLength = 8; // Longitud mínima de la contraseña
  options.Password.RequiredUniqueChars = 1; // Número de caracteres únicos requeridos

  // Opciones de Lockout (bloqueo de cuenta tras intentos fallidos)
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de bloqueo
  options.Lockout.MaxFailedAccessAttempts = 5; // Intentos fallidos antes de bloquear
  options.Lockout.AllowedForNewUsers = true;

  // Opciones de Usuario
  options.User.AllowedUserNameCharacters =
      "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Caracteres permitidos en nombres de usuario
  options.User.RequireUniqueEmail = true; // Requerir que los correos electrónicos sean únicos

  // Opciones de SignIn (Inicio de Sesión)
  // options.SignIn.RequireConfirmedEmail = false; // Puedes ponerlo en true si implementas confirmación de email
  // options.SignIn.RequireConfirmedPhoneNumber = false; // Si usas confirmación por teléfono
})
    .AddEntityFrameworkStores<VNCenterDbContext>()
    .AddDefaultTokenProviders(); // Para tokens de reseteo de contraseña, email, etc.

// 3. Configuración de la Cookie de Autenticación
// Esto configura cómo la aplicación maneja la autenticación basada en cookies.
builder.Services.ConfigureApplicationCookie(options =>
{
  // Opciones de la Cookie
  options.Cookie.HttpOnly = true; // La cookie no es accesible por script del lado del cliente
  options.ExpireTimeSpan = TimeSpan.FromDays(30); // Duración de la cookie de sesión

  options.LoginPath = "/Auth/LoginBasic"; // Ruta a la página de inicio de sesión (ajusta si tu controlador/acción es diferente)
  options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta si el acceso es denegado (ajusta si es diferente)
  options.SlidingExpiration = true; // La cookie se renueva si el usuario está activo
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. Añadir Middleware de Autenticación y Autorización
// ¡IMPORTANTE! Deben ir DESPUÉS de UseRouting y ANTES de UseEndpoints (que es MapControllerRoute)
app.UseAuthentication(); // Habilita la autenticación
app.UseAuthorization();  // Habilita la autorización

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

app.Run();
