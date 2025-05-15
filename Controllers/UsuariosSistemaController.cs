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

    public UsuariosSistemaController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        RoleManager<RolesSistema> roleManager,
        IWebHostEnvironment webHostEnvironment,
        IAuditoriaService auditoriaService,
        IHttpContextAccessor httpContextAccessor)
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _webHostEnvironment = webHostEnvironment;
      _auditoriaService = auditoriaService;
      _httpContextAccessor = httpContextAccessor;
    }

    private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private async Task<string?> GetCurrentUserFullNameAsync()
    {
      var user = await _userManager.GetUserAsync(User);
      return user?.NombreCompleto;
    }
    private string? GetUserIpAddress() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    // GET: UsuariosSistema
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
        return NotFound();
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
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
    public async Task<IActionResult> Create()
    {
      ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name");
      return View(new UsuarioCreateViewModel());
    }

    // POST: UsuariosSistema/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
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
          EmailConfirmed = true,
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
        return NotFound();
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
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

      // No validar NewPassword y ConfirmNewPassword si están vacíos,
      // ya que el cambio de contraseña es opcional.
      // La validación de [Compare] y [StringLength] se activará solo si NewPassword tiene valor.
      if (string.IsNullOrEmpty(viewModel.NewPassword) && string.IsNullOrEmpty(viewModel.ConfirmNewPassword))
      {
        ModelState.Remove(nameof(viewModel.NewPassword));
        ModelState.Remove(nameof(viewModel.ConfirmNewPassword));
      }

      if (ModelState.IsValid)
      {
        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario == null)
        {
          return NotFound();
        }

        var cambios = new StringBuilder();
        // Comparar y registrar cambios en propiedades
        if (usuario.UserName != viewModel.UserName) { cambios.AppendLine($"UserName: de '{usuario.UserName}' a '{viewModel.UserName}'."); usuario.UserName = viewModel.UserName; }
        if (usuario.Email != viewModel.Email) { cambios.AppendLine($"Email: de '{usuario.Email}' a '{viewModel.Email}'."); usuario.Email = viewModel.Email; }
        if (usuario.Nombres != viewModel.Nombres) { cambios.AppendLine($"Nombres: de '{usuario.Nombres}' a '{viewModel.Nombres}'."); usuario.Nombres = viewModel.Nombres; }
        if (usuario.Apellidos != viewModel.Apellidos) { cambios.AppendLine($"Apellidos: de '{usuario.Apellidos}' a '{viewModel.Apellidos}'."); usuario.Apellidos = viewModel.Apellidos; }
        if (usuario.Activo != viewModel.Activo) { cambios.AppendLine($"Activo: de '{usuario.Activo}' a '{viewModel.Activo}'."); usuario.Activo = viewModel.Activo; }
        if (usuario.PhoneNumber != viewModel.PhoneNumber) { cambios.AppendLine($"Teléfono: de '{usuario.PhoneNumber ?? "N/A"}' a '{viewModel.PhoneNumber ?? "N/A"}'."); usuario.PhoneNumber = viewModel.PhoneNumber; }

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
          // Manejar cambio de rol
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
                // Repopular y retornar si hay error de rol
                ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
                return View(viewModel);
              }
            }
            else if (!string.IsNullOrEmpty(oldRoleName)) // Si se quitó el rol
            {
              cambios.AppendLine($"Rol: de '{oldRoleName}' a 'Ninguno'.");
            }
          }

          // Manejar restablecimiento de contraseña
          if (!string.IsNullOrEmpty(viewModel.NewPassword))
          {
            // Validar que NewPassword y ConfirmNewPassword coincidan (ya lo hace [Compare], pero una comprobación extra no daña)
            if (viewModel.NewPassword != viewModel.ConfirmNewPassword)
            {
              ModelState.AddModelError("ConfirmNewPassword", "La nueva contraseña y la contraseña de confirmación no coinciden.");
            }
            else
            {
              // Quitar contraseña anterior y añadir la nueva
              var removePasswordResult = await _userManager.RemovePasswordAsync(usuario);
              if (removePasswordResult.Succeeded)
              {
                var addPasswordResult = await _userManager.AddPasswordAsync(usuario, viewModel.NewPassword);
                if (addPasswordResult.Succeeded)
                {
                  cambios.AppendLine("Contraseña restablecida por administrador.");
                }
                else
                {
                  foreach (var error in addPasswordResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
                }
              }
              else // Si el usuario no tenía contraseña (ej. login externo), AddPasswordAsync podría ser suficiente
              {
                var addPasswordResult = await _userManager.AddPasswordAsync(usuario, viewModel.NewPassword);
                if (addPasswordResult.Succeeded)
                {
                  cambios.AppendLine("Contraseña establecida por administrador (usuario no tenía contraseña previa).");
                }
                else
                {
                  // Podría ser que el usuario SÍ tenía contraseña pero RemovePasswordAsync falló por alguna razón
                  // O que AddPasswordAsync falló por políticas de contraseña.
                  foreach (var error in addPasswordResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
                }
              }
            }
          }

          // Si hay errores después de intentar cambiar contraseña, retornar
          if (!ModelState.IsValid)
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

    // GET: UsuariosSistema/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      // ... (código existente de Delete GET) ...
      if (id == null)
      {
        return NotFound();
      }
      var usuario = await _userManager.FindByIdAsync(id.Value.ToString());
      if (usuario == null)
      {
        return NotFound();
      }
      ViewBag.UserRoles = await _userManager.GetRolesAsync(usuario);
      return View(usuario);
    }

    // POST: UsuariosSistema/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      // ... (código existente de Delete POST con auditoría) ...
      var usuario = await _userManager.FindByIdAsync(id.ToString());
      if (usuario != null)
      {
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

    public async Task<IActionResult> ExportToPdf()
    {
      // ... (código existente de ExportToPdf) ...
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
