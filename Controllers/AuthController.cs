using Microsoft.AspNetCore.Mvc;
using VN_Center.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using VN_Center.Models.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using VN_Center.Services;

namespace VN_Center.Controllers
{
  public class AuthController : Controller
  {
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly SignInManager<UsuariosSistema> _signInManager;
    private readonly RoleManager<RolesSistema> _roleManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IEmailSender _emailSender;

    public AuthController(
        UserManager<UsuariosSistema> userManager,
        SignInManager<UsuariosSistema> signInManager,
        RoleManager<RolesSistema> roleManager,
        ILogger<AuthController> logger,
        IEmailSender emailSender)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager;
      _logger = logger;
      _emailSender = emailSender; // *** ASIGNAR IEmailSender ***
    }

    // ... (LoginBasic, RegisterBasic, Logout, AccessDenied sin cambios) ...

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
      return RedirectToAction(nameof(LoginBasic), "Auth");
    }

    // GET: /Auth/AccessDenied
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
      return View();
    }

    // GET: /Auth/ForgotPasswordBasic
    [AllowAnonymous]
    public IActionResult ForgotPasswordBasic()
    {
      return View();
    }

    // POST: /Auth/ForgotPasswordBasic
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPasswordBasic(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null /* || !(await _userManager.IsEmailConfirmedAsync(user)) */)
        {
          _logger.LogWarning($"Intento de reseteo de contraseña para email no existente o no confirmado: {model.Email}");
          return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = user.Email, code = code }, protocol: Request.Scheme);

        _logger.LogInformation($"Token de reseteo de contraseña generado para {user.Email}.");

        // *** MODIFICACIÓN AQUÍ: Pasamos solo el callbackUrl como 'htmlMessageAsResetLink' ***
        await _emailSender.SendEmailAsync(
            model.Email,
            "Restablecer Contraseña - VN Center", // Este asunto se puede definir/sobrescribir en la plantilla de SendGrid
            callbackUrl! // SendGridEmailSender usará esto para el campo {{{reset_link}}}
        );

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
      }

      return View(model);
    }

    // GET: /Auth/ForgotPasswordConfirmation
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
      // Ya no se muestra el enlace aquí, se asume que se envió por correo.
      return View();
    }

    // GET: /Auth/ResetPassword
    [AllowAnonymous]
    public IActionResult ResetPassword(string? code = null, string? email = null)
    {
      if (code == null || email == null)
      {
        return BadRequest("Se requiere un código y un correo electrónico para restablecer la contraseña.");
      }
      var model = new ResetPasswordViewModel { Token = code, Email = email };
      return View(model);
    }

    // POST: /Auth/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var user = await _userManager.FindByEmailAsync(model.Email);
      if (user == null)
      {
        _logger.LogWarning($"Intento de reseteo de contraseña para usuario no existente: {model.Email}");
        return RedirectToAction(nameof(ResetPasswordConfirmation));
      }

      try
      {
        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
        if (result.Succeeded)
        {
          _logger.LogInformation($"Contraseña restablecida exitosamente para el usuario {user.Email}.");
          return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        _logger.LogWarning($"Error al restablecer contraseña para {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      catch (FormatException)
      {
        _logger.LogWarning($"Token de reseteo de contraseña malformado para {model.Email}.");
        ModelState.AddModelError(string.Empty, "El token de reseteo de contraseña no es válido.");
      }

      return View(model);
    }

    // GET: /Auth/ResetPasswordConfirmation
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
      return View();
    }
  }
}
