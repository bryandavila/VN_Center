using Microsoft.AspNetCore.Mvc;
using VN_Center.Models.ViewModels; // Para LoginViewModel y RegisterViewModel
using Microsoft.AspNetCore.Identity;
using VN_Center.Models.Entities; // Para UsuariosSistema
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [AllowAnonymous]
using Microsoft.Extensions.Logging; // Para ILogger

namespace VN_Center.Controllers
{
  public class AuthController : Controller
  {
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly SignInManager<UsuariosSistema> _signInManager;
    private readonly RoleManager<RolesSistema> _roleManager; // *** AÑADIDO CAMPO PARA RoleManager ***
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<UsuariosSistema> userManager,
        SignInManager<UsuariosSistema> signInManager,
        RoleManager<RolesSistema> roleManager, // *** AÑADIDA INYECCIÓN DE RoleManager ***
        ILogger<AuthController> logger)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager; // *** ASIGNAR RoleManager INYECTADO ***
      _logger = logger;
    }

    // GET: /Auth/LoginBasic
    [AllowAnonymous]
    public IActionResult LoginBasic(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    // POST: /Auth/LoginBasic
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginBasic(LoginViewModel model, string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      returnUrl ??= Url.Content("~/Dashboards/Index");

      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByNameAsync(model.EmailOrUserName);
        if (user == null)
        {
          user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
        }

        if (user != null)
        {
          if (!user.Activo)
          {
            ModelState.AddModelError(string.Empty, "Su cuenta está desactivada.");
            return View(model);
          }

          var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
          if (result.Succeeded)
          {
            _logger.LogInformation($"Usuario '{user.UserName}' inició sesión.");
            user.FechaUltimoAcceso = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return LocalRedirect(returnUrl);
          }
          if (result.IsLockedOut)
          {
            _logger.LogWarning($"Cuenta de usuario '{user.UserName}' bloqueada.");
            ModelState.AddModelError(string.Empty, "Su cuenta ha sido bloqueada debido a demasiados intentos fallidos. Por favor, intente más tarde.");
            return View(model);
          }
          if (result.IsNotAllowed)
          {
            ModelState.AddModelError(string.Empty, "Inicio de sesión no permitido. Verifique su correo electrónico si la confirmación es necesaria.");
            return View(model);
          }
        }

        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
        return View(model);
      }
      return View(model);
    }

    // GET: /Auth/RegisterBasic
    [AllowAnonymous]
    public IActionResult RegisterBasic()
    {
      return View();
    }

    // POST: /Auth/RegisterBasic
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterBasic(RegisterViewModel model, string? returnUrl = null)
    {
      returnUrl ??= Url.Content("~/Dashboards/Index");
      if (ModelState.IsValid)
      {
        var user = new UsuariosSistema
        {
          UserName = model.UserName,
          Email = model.Email,
          Nombres = model.Nombres,
          Apellidos = model.Apellidos,
          PhoneNumber = model.PhoneNumber,
          Activo = true,
          EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          _logger.LogInformation($"Usuario '{user.UserName}' creó una nueva cuenta con contraseña.");

          // Asignar el rol "Usuario" por defecto
          // Ahora _roleManager está disponible
          if (await _roleManager.RoleExistsAsync("Usuario"))
          {
            await _userManager.AddToRoleAsync(user, "Usuario");
            _logger.LogInformation($"Usuario '{user.UserName}' asignado al rol 'Usuario'.");
          }
          else
          {
            _logger.LogWarning("Rol 'Usuario' no encontrado. El nuevo usuario no fue asignado a un rol.");
          }

          TempData["SuccessMessage"] = "¡Registro exitoso! Ahora puede iniciar sesión.";
          return RedirectToAction(nameof(LoginBasic));
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      return View(model);
    }


    // POST: /Auth/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      _logger.LogInformation("Usuario cerró sesión.");
      return RedirectToAction("LoginBasic", "Auth"); // Redirigir a la página de Login después de cerrar sesión
    }

    // GET: /Auth/AccessDenied
    [AllowAnonymous] // Permitir acceso anónimo a la página de acceso denegado
    public IActionResult AccessDenied()
    {
      return View(); // Asegúrate de tener una vista Views/Auth/AccessDenied.cshtml
    }

    // TODO: Implementar ForgotPassword y ResetPassword si es necesario
    // GET: /Auth/ForgotPasswordBasic
    // [AllowAnonymous]
    // public IActionResult ForgotPasswordBasic()
    // {
    //     return View();
    // }

    // POST: /Auth/ForgotPasswordBasic
    // [HttpPost]
    // [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> ForgotPasswordBasic(ForgotPasswordViewModel model)
    // {
    //     // Lógica para enviar email de reseteo de contraseña
    // }
  }
}
