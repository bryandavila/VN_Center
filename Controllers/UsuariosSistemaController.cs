// VN_Center/Controllers/UsuariosSistemaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using VN_Center.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using VN_Center.Documents;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using VN_Center.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text; // Para StringBuilder en auditoría

namespace VN_Center.Controllers
{
  [Authorize]
  public class UsuariosSistemaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly RoleManager<RolesSistema> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UsuariosSistemaController> _logger; // Agregado para logging

    public UsuariosSistemaController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        RoleManager<RolesSistema> roleManager,
        IWebHostEnvironment webHostEnvironment,
        IAuditoriaService auditoriaService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UsuariosSistemaController> logger) // Inyectar ILogger
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _webHostEnvironment = webHostEnvironment;
      _auditoriaService = auditoriaService;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger; // Asignar logger
    }

    private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private async Task<string?> GetCurrentUserFullNameAsync()
    {
      var user = await _userManager.GetUserAsync(User);
      return user?.NombreCompleto;
    }
    private string? GetUserIpAddress() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    // GET: UsuariosSistema
    [Authorize(Roles = "Administrador")] // Solo administradores pueden listar todos los usuarios
    public async Task<IActionResult> Index()
    {
      var usuarios = await _userManager.Users.OrderBy(u => u.Nombres).ThenBy(u => u.Apellidos).ToListAsync();
      var viewModelList = new List<UsuarioSistemaViewModel>();

      foreach (var usuario in usuarios)
      {
        viewModelList.Add(new UsuarioSistemaViewModel
        {
          Id = usuario.Id.ToString(),
          UserName = usuario.UserName,
          Nombres = usuario.Nombres,
          Apellidos = usuario.Apellidos,
          NombreCompleto = usuario.NombreCompleto,
          Email = usuario.Email,
          EmailConfirmed = usuario.EmailConfirmed,
          Activo = usuario.Activo,
          IsLockedOut = usuario.LockoutEnd.HasValue && usuario.LockoutEnd.Value > DateTimeOffset.UtcNow,
          LockoutEndDateUtc = usuario.LockoutEnd,
          Roles = await _userManager.GetRolesAsync(usuario)
        });
      }
      return View(viewModelList);
    }

    // GET: UsuariosSistema/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        TempData["ErrorMessage"] = "ID de usuario no proporcionado.";
        return RedirectToAction("Index", "Home"); // O a una página de error apropiada
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        TempData["ErrorMessage"] = "Usuario no encontrado.";
        return NotFound();
      }

      // Autorización: Solo el administrador o el propio usuario pueden ver los detalles.
      var currentUserId = GetCurrentUserId();
      if (!User.IsInRole("Administrador") && usuario.Id.ToString() != currentUserId)
      {
        _logger.LogWarning($"Acceso denegado a Details para Usuario ID: {id} por Usuario ID: {currentUserId}.");
        TempData["ErrorMessage"] = "No tiene permiso para ver los detalles de este usuario.";
        // Redirigir al dashboard o a una página de acceso denegado específica para usuarios no admin
        return RedirectToAction("Index", "Dashboards");
      }

      var roles = await _userManager.GetRolesAsync(usuario);
      var viewModel = new UsuarioSistemaViewModel
      {
        Id = usuario.Id.ToString(),
        UserName = usuario.UserName,
        Nombres = usuario.Nombres,
        Apellidos = usuario.Apellidos,
        NombreCompleto = usuario.NombreCompleto,
        Email = usuario.Email,
        EmailConfirmed = usuario.EmailConfirmed,
        Activo = usuario.Activo,
        IsLockedOut = usuario.LockoutEnd.HasValue && usuario.LockoutEnd.Value > DateTimeOffset.UtcNow,
        LockoutEndDateUtc = usuario.LockoutEnd,
        Roles = roles
      };
      return View(viewModel);
    }

    // GET: UsuariosSistema/Create
    [Authorize(Roles = "Administrador")] // Solo administradores pueden crear usuarios
    public async Task<IActionResult> Create()
    {
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name");
      return View(new UsuarioCreateViewModel());
    }

    // POST: UsuariosSistema/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo administradores pueden crear usuarios
    public async Task<IActionResult> Create(UsuarioCreateViewModel viewModel)
    {
      if (ModelState.IsValid)
      {
        var usuario = new UsuariosSistema
        {
          UserName = viewModel.UserName,
          Email = viewModel.Email,
          Nombres = viewModel.Nombres,
          Apellidos = viewModel.Apellidos,
          Activo = viewModel.Activo,
          EmailConfirmed = true, // O manejar la confirmación por email si se implementa
          PhoneNumber = viewModel.PhoneNumber,
        };
        var result = await _userManager.CreateAsync(usuario, viewModel.Password);
        if (result.Succeeded)
        {
          string detallesAuditoria = $"Usuario '{usuario.UserName}' creado. Nombres: {usuario.Nombres}, Apellidos: {usuario.Apellidos}, Email: {usuario.Email}, Activo: {usuario.Activo}.";
          if (!string.IsNullOrEmpty(viewModel.SelectedRoleName))
          {
            if (await _roleManager.RoleExistsAsync(viewModel.SelectedRoleName))
            {
              await _userManager.AddToRoleAsync(usuario, viewModel.SelectedRoleName);
              detallesAuditoria += $" Asignado al rol: '{viewModel.SelectedRoleName}'.";
            }
            else
            {
              ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
              ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
              return View(viewModel);
            }
          }
          await _auditoriaService.RegistrarEventoAuditoriaAsync(
              GetCurrentUserId(),
              await GetCurrentUserFullNameAsync(),
              "CreacionUsuario",
              "UsuariosSistema",
              usuario.Id.ToString(),
              detallesAuditoria,
              GetUserIpAddress()
          );
          TempData["SuccessMessage"] = "Usuario del sistema creado exitosamente.";
          return RedirectToAction(nameof(Index));
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      return View(viewModel);
    }

    // GET: UsuariosSistema/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        TempData["ErrorMessage"] = "ID de usuario no proporcionado.";
        return RedirectToAction("Index", "Home");
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        TempData["ErrorMessage"] = "Usuario no encontrado.";
        return NotFound();
      }

      // Autorización: Solo el administrador o el propio usuario pueden editar.
      var currentUserId = GetCurrentUserId();
      if (!User.IsInRole("Administrador") && usuario.Id.ToString() != currentUserId)
      {
        _logger.LogWarning($"Acceso denegado a Edit GET para Usuario ID: {id} por Usuario ID: {currentUserId}.");
        TempData["ErrorMessage"] = "No tiene permiso para editar este usuario.";
        // Redirigir al dashboard o a una página de acceso denegado específica para usuarios no admin
        return RedirectToAction("Details", "UsuariosSistema", new { id = currentUserId });
      }

      var userRoles = await _userManager.GetRolesAsync(usuario);
      var currentRoleName = userRoles.FirstOrDefault();
      var viewModel = new UsuarioEditViewModel
      {
        Id = usuario.Id,
        UserName = usuario.UserName,
        Email = usuario.Email,
        Nombres = usuario.Nombres,
        Apellidos = usuario.Apellidos,
        Activo = usuario.Activo,
        PhoneNumber = usuario.PhoneNumber,
        SelectedRoleName = currentRoleName
      };
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      return View(viewModel);
    }

    // POST: UsuariosSistema/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioEditViewModel viewModel)
    {
      if (id != viewModel.Id)
      {
        return NotFound();
      }

      var usuario = await _userManager.FindByIdAsync(id.ToString());
      if (usuario == null)
      {
        TempData["ErrorMessage"] = "Usuario no encontrado.";
        return NotFound();
      }

      // Autorización: Solo el administrador o el propio usuario pueden procesar la edición.
      var currentUserId = GetCurrentUserId();
      bool isCurrentUserAdmin = User.IsInRole("Administrador");

      if (!isCurrentUserAdmin && usuario.Id.ToString() != currentUserId)
      {
        _logger.LogWarning($"Intento de edición no autorizado para Usuario ID: {id} por Usuario ID: {currentUserId}.");
        TempData["ErrorMessage"] = "No tiene permiso para modificar este usuario.";
        return RedirectToAction("Details", "UsuariosSistema", new { id = currentUserId });
      }

      // Si el usuario no es administrador, no puede cambiar su propio rol ni su estado de activación.
      // Esos valores se tomarán del usuario existente en la BD.
      if (!isCurrentUserAdmin)
      {
        viewModel.SelectedRoleName = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault();
        viewModel.Activo = usuario.Activo;
      }

      // No validar NewPassword y ConfirmNewPassword si están vacíos,
      // ya que el cambio de contraseña es opcional.
      if (string.IsNullOrEmpty(viewModel.NewPassword) && string.IsNullOrEmpty(viewModel.ConfirmNewPassword))
      {
        ModelState.Remove(nameof(viewModel.NewPassword));
        ModelState.Remove(nameof(viewModel.ConfirmNewPassword));
      }

      if (ModelState.IsValid)
      {
        var cambios = new StringBuilder();
        // Comparar y registrar cambios en propiedades
        if (usuario.UserName != viewModel.UserName) { cambios.AppendLine($"UserName: de '{usuario.UserName}' a '{viewModel.UserName}'."); usuario.UserName = viewModel.UserName; }
        if (usuario.Email != viewModel.Email) { cambios.AppendLine($"Email: de '{usuario.Email}' a '{viewModel.Email}'."); usuario.Email = viewModel.Email; }
        if (usuario.Nombres != viewModel.Nombres) { cambios.AppendLine($"Nombres: de '{usuario.Nombres}' a '{viewModel.Nombres}'."); usuario.Nombres = viewModel.Nombres; }
        if (usuario.Apellidos != viewModel.Apellidos) { cambios.AppendLine($"Apellidos: de '{usuario.Apellidos}' a '{viewModel.Apellidos}'."); usuario.Apellidos = viewModel.Apellidos; }
        if (usuario.PhoneNumber != viewModel.PhoneNumber) { cambios.AppendLine($"Teléfono: de '{usuario.PhoneNumber ?? "N/A"}' a '{viewModel.PhoneNumber ?? "N/A"}'."); usuario.PhoneNumber = viewModel.PhoneNumber; }

        // Solo el administrador puede cambiar el estado 'Activo'
        if (isCurrentUserAdmin && usuario.Activo != viewModel.Activo)
        {
          cambios.AppendLine($"Activo: de '{usuario.Activo}' a '{viewModel.Activo}'.");
          usuario.Activo = viewModel.Activo;
        }

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
          // Manejar cambio de rol (solo si es administrador)
          if (isCurrentUserAdmin)
          {
            var currentRoles = await _userManager.GetRolesAsync(usuario);
            var oldRoleName = currentRoles.FirstOrDefault();
            var newRoleName = viewModel.SelectedRoleName;

            if (oldRoleName != newRoleName)
            {
              if (!string.IsNullOrEmpty(oldRoleName))
              {
                await _userManager.RemoveFromRoleAsync(usuario, oldRoleName);
              }
              if (!string.IsNullOrEmpty(newRoleName))
              {
                if (await _roleManager.RoleExistsAsync(newRoleName))
                {
                  await _userManager.AddToRoleAsync(usuario, newRoleName);
                  cambios.AppendLine($"Rol: de '{oldRoleName ?? "Ninguno"}' a '{newRoleName}'.");
                }
                else
                {
                  ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
                  ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
                  return View(viewModel);
                }
              }
              else if (!string.IsNullOrEmpty(oldRoleName))
              {
                cambios.AppendLine($"Rol: de '{oldRoleName}' a 'Ninguno'.");
              }
            }
          }

          // Manejar restablecimiento de contraseña
          if (!string.IsNullOrEmpty(viewModel.NewPassword))
          {
            if (viewModel.NewPassword != viewModel.ConfirmNewPassword)
            {
              ModelState.AddModelError("ConfirmNewPassword", "La nueva contraseña y la contraseña de confirmación no coinciden.");
            }
            else
            {
              var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
              var resetPassResult = await _userManager.ResetPasswordAsync(usuario, token, viewModel.NewPassword);
              if (resetPassResult.Succeeded)
              {
                cambios.AppendLine("Contraseña restablecida.");
              }
              else
              {
                foreach (var error in resetPassResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
              }
            }
          }

          if (!ModelState.IsValid) // Si hubo errores al cambiar contraseña
          {
            ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
            return View(viewModel);
          }

          if (cambios.Length > 0)
          {
            await _auditoriaService.RegistrarEventoAuditoriaAsync(
                GetCurrentUserId(),
                await GetCurrentUserFullNameAsync(),
                "ActualizacionUsuario",
                "UsuariosSistema",
                usuario.Id.ToString(),
                $"Usuario '{usuario.UserName}' actualizado. Cambios: {cambios.ToString().Trim()}",
                GetUserIpAddress()
            );
          }

          TempData["SuccessMessage"] = "Usuario del sistema actualizado exitosamente.";

          // *** LÓGICA DE REDIRECCIÓN MODIFICADA ***
          if (isCurrentUserAdmin)
          {
            return RedirectToAction(nameof(Index));
          }
          else
          {
            // Redirigir al usuario normal a los detalles de su propio perfil
            return RedirectToAction(nameof(Details), new { id = usuario.Id });
          }
        }
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
      return View(viewModel);
    }

    // GET: UsuariosSistema/Delete/5
    [Authorize(Roles = "Administrador")] // Solo administradores pueden eliminar
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
      }
      ViewBag.UserRoles = await _userManager.GetRolesAsync(usuario); // Para mostrar en la vista de confirmación
      return View(usuario);
    }

    // POST: UsuariosSistema/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo administradores pueden eliminar
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var usuario = await _userManager.FindByIdAsync(id.ToString());
      if (usuario != null)
      {
        // No permitir que el admin se elimine a sí mismo si es el único admin (lógica adicional podría ser necesaria)
        var currentUserId = GetCurrentUserId();
        if (usuario.Id.ToString() == currentUserId && User.IsInRole("Administrador"))
        {
          var adminCount = (await _userManager.GetUsersInRoleAsync("Administrador")).Count;
          if (adminCount <= 1)
          {
            TempData["ErrorMessage"] = "No puede eliminar al único administrador del sistema.";
            return RedirectToAction(nameof(Index));
          }
        }

        var userNameParaAuditoria = usuario.UserName;
        var userIdParaAuditoria = usuario.Id.ToString();

        var result = await _userManager.DeleteAsync(usuario);
        if (result.Succeeded)
        {
          await _auditoriaService.RegistrarEventoAuditoriaAsync(
              GetCurrentUserId(),
              await GetCurrentUserFullNameAsync(),
              "EliminacionUsuario",
              "UsuariosSistema",
              userIdParaAuditoria,
              $"Usuario '{userNameParaAuditoria}' (ID: {userIdParaAuditoria}) eliminado.",
              GetUserIpAddress()
          );
          TempData["SuccessMessage"] = "Usuario del sistema eliminado exitosamente.";
        }
        else
        {
          TempData["ErrorMessage"] = "No se pudo eliminar el usuario. " + string.Join(" ", result.Errors.Select(e => e.Description));
        }
      }
      else
      {
        TempData["ErrorMessage"] = "No se encontró el usuario a eliminar.";
      }
      return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExportToPdf()
    {
      var users = await _userManager.Users.OrderBy(u => u.Nombres).ThenBy(u => u.Apellidos).ToListAsync();
      var usuariosParaPdf = new List<UsuarioSistemaViewModel>();

      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);
        usuariosParaPdf.Add(new UsuarioSistemaViewModel
        {
          Id = user.Id.ToString(),
          UserName = user.UserName,
          Nombres = user.Nombres,
          Apellidos = user.Apellidos,
          NombreCompleto = user.NombreCompleto,
          Email = user.Email,
          EmailConfirmed = user.EmailConfirmed,
          Activo = user.Activo,
          IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow,
          LockoutEndDateUtc = user.LockoutEnd,
          Roles = roles
        });
      }

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new UsuariosSistemaPdfDocument(usuariosParaPdf, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Lista_Usuarios_Sistema_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }
  }
}
