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
using VN_Center.Services; // <--- AÑADIDO: Para IAuditoriaService
using System.Security.Claims; // <--- AÑADIDO: Para obtener el ID del usuario actual
using Microsoft.AspNetCore.Http; // <--- AÑADIDO: Para IHttpContextAccessor

namespace VN_Center.Controllers
{
  [Authorize]
  public class UsuariosSistemaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly RoleManager<RolesSistema> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IAuditoriaService _auditoriaService; // <--- AÑADIDO: Servicio de Auditoría
    private readonly IHttpContextAccessor _httpContextAccessor; // <--- AÑADIDO: Para obtener IP

    public UsuariosSistemaController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        RoleManager<RolesSistema> roleManager,
        IWebHostEnvironment webHostEnvironment,
        IAuditoriaService auditoriaService, // <--- AÑADIDO
        IHttpContextAccessor httpContextAccessor) // <--- AÑADIDO
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _webHostEnvironment = webHostEnvironment;
      _auditoriaService = auditoriaService; // <--- AÑADIDO
      _httpContextAccessor = httpContextAccessor; // <--- AÑADIDO
    }

    private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private string? GetCurrentUserFullName()
    {
      var user = _userManager.GetUserAsync(User).Result; // Sincrónico, usar con cuidado o pasar HttpContext
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

          // Registrar evento de auditoría
          await _auditoriaService.RegistrarEventoAuditoriaAsync(
              GetCurrentUserId(),
              GetCurrentUserFullName(),
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

      if (ModelState.IsValid)
      {
        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario == null)
        {
          return NotFound();
        }

        // Guardar estado anterior para auditoría
        var oldEmail = usuario.Email;
        var oldNombres = usuario.Nombres;
        var oldApellidos = usuario.Apellidos;
        var oldActivo = usuario.Activo;
        var oldPhoneNumber = usuario.PhoneNumber;
        var oldUserName = usuario.UserName;
        var oldRoles = await _userManager.GetRolesAsync(usuario);
        var oldRoleName = oldRoles.FirstOrDefault();

        // Actualizar propiedades
        usuario.UserName = viewModel.UserName;
        usuario.Email = viewModel.Email;
        usuario.Nombres = viewModel.Nombres;
        usuario.Apellidos = viewModel.Apellidos;
        usuario.Activo = viewModel.Activo;
        usuario.PhoneNumber = viewModel.PhoneNumber;

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
          var cambios = new List<string>();
          if (oldUserName != usuario.UserName) cambios.Add($"UserName: de '{oldUserName}' a '{usuario.UserName}'");
          if (oldEmail != usuario.Email) cambios.Add($"Email: de '{oldEmail}' a '{usuario.Email}'");
          if (oldNombres != usuario.Nombres) cambios.Add($"Nombres: de '{oldNombres}' a '{usuario.Nombres}'");
          if (oldApellidos != usuario.Apellidos) cambios.Add($"Apellidos: de '{oldApellidos}' a '{usuario.Apellidos}'");
          if (oldActivo != usuario.Activo) cambios.Add($"Activo: de '{oldActivo}' a '{usuario.Activo}'");
          if (oldPhoneNumber != usuario.PhoneNumber) cambios.Add($"Teléfono: de '{oldPhoneNumber ?? "N/A"}' a '{usuario.PhoneNumber ?? "N/A"}'");

          // Actualizar rol
          var newRoleName = viewModel.SelectedRoleName;
          if (oldRoleName != newRoleName)
          {
            if (oldRoles.Any())
            {
              await _userManager.RemoveFromRolesAsync(usuario, oldRoles);
            }
            if (!string.IsNullOrEmpty(newRoleName))
            {
              if (await _roleManager.RoleExistsAsync(newRoleName))
              {
                await _userManager.AddToRoleAsync(usuario, newRoleName);
                cambios.Add($"Rol: de '{oldRoleName ?? "Ninguno"}' a '{newRoleName}'");
              }
              else
              {
                ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
                ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
                return View(viewModel);
              }
            }
            else if (oldRoles.Any()) // Si se quitó el rol
            {
              cambios.Add($"Rol: de '{oldRoleName}' a 'Ninguno'");
            }
          }

          if (cambios.Any())
          {
            await _auditoriaService.RegistrarEventoAuditoriaAsync(
                GetCurrentUserId(),
                GetCurrentUserFullName(),
                "ActualizacionUsuario",
                "UsuariosSistema",
                usuario.Id.ToString(),
                $"Usuario '{usuario.UserName}' actualizado. Cambios: {string.Join("; ", cambios)}.",
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
      var usuario = await _userManager.FindByIdAsync(id.ToString());
      if (usuario != null)
      {
        var userNameParaAuditoria = usuario.UserName; // Guardar antes de eliminar
        var userIdParaAuditoria = usuario.Id.ToString();

        var result = await _userManager.DeleteAsync(usuario);
        if (result.Succeeded)
        {
          await _auditoriaService.RegistrarEventoAuditoriaAsync(
              GetCurrentUserId(),
              GetCurrentUserFullName(),
              "EliminacionUsuario",
              "UsuariosSistema",
              userIdParaAuditoria, // Usar el ID guardado
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
