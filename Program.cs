using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using VN_Center.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// *** ELIMINADA LA CONFIGURACIÓN DE LICENCIA DE QUESTPDF ***
// QuestPDF.Settings.License = LicenseType.Community; 

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<VNCenterDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<UsuariosSistema, RolesSistema>(options => {
  options.SignIn.RequireConfirmedAccount = false;
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 8;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = true;
  options.Password.RequireLowercase = true;
  options.Password.RequiredUniqueChars = 1;
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;
  options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
  options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<VNCenterDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly = true;
  options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
  options.LoginPath = "/Auth/LoginBasic";
  options.LogoutPath = "/Auth/Logout";
  options.AccessDeniedPath = "/Auth/AccessDenied";
  options.SlidingExpiration = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
  try
  {
    await DataSeeder.Initialize(app.Services, app.Configuration);
    Console.WriteLine("Data seeding completed or verified.");
  }
  catch (Exception ex)
  {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB.");
    Console.WriteLine($"An error occurred seeding the DB: {ex.Message} \n {ex.StackTrace}");
  }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
