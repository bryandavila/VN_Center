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
using Microsoft.AspNetCore.Hosting; // Asegúrate que este using esté presente
using System.IO;

namespace VN_Center.Controllers
{
  [Authorize]
  public class UsuariosSistemaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly RoleManager<RolesSistema> _roleManager;
    private readonly IWebHostEnvironment _webHostEnvironment; // Declaración del campo

    public UsuariosSistemaController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        RoleManager<RolesSistema> roleManager,
        IWebHostEnvironment webHostEnvironment) // Asegúrate que IWebHostEnvironment se inyecte
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
      _webHostEnvironment = webHostEnvironment; // Inicialización
    }

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
        // PhoneNumber = usuario.PhoneNumber, // Asegúrate que PhoneNumber exista en tu entidad UsuariosSistema si lo usas
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
          if (!string.IsNullOrEmpty(viewModel.SelectedRoleName))
          {
            if (await _roleManager.RoleExistsAsync(viewModel.SelectedRoleName))
            {
              await _userManager.AddToRoleAsync(usuario, viewModel.SelectedRoleName);
            }
            else
            {
              ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
              ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
              return View(viewModel);
            }
          }
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

        usuario.UserName = viewModel.UserName;
        usuario.Email = viewModel.Email;
        usuario.Nombres = viewModel.Nombres;
        usuario.Apellidos = viewModel.Apellidos;
        usuario.Activo = viewModel.Activo;
        usuario.PhoneNumber = viewModel.PhoneNumber;

        var result = await _userManager.UpdateAsync(usuario);

        if (result.Succeeded)
        {
          var currentRoles = await _userManager.GetRolesAsync(usuario);
          var newRoleName = viewModel.SelectedRoleName;

          if (!currentRoles.Contains(newRoleName ?? string.Empty) || (currentRoles.Any() && string.IsNullOrEmpty(newRoleName)))
          {
            if (currentRoles.Any())
            {
              await _userManager.RemoveFromRolesAsync(usuario, currentRoles);
            }
            if (!string.IsNullOrEmpty(newRoleName))
            {
              if (await _roleManager.RoleExistsAsync(newRoleName))
              {
                await _userManager.AddToRoleAsync(usuario, newRoleName);
              }
              else
              {
                ModelState.AddModelError("SelectedRoleName", "El rol seleccionado no es válido.");
                ViewData["RolesList"] = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Name", "Name", viewModel.SelectedRoleName);
                return View(viewModel);
              }
            }
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
        var result = await _userManager.DeleteAsync(usuario);
        if (result.Succeeded)
        {
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

    // ACCIÓN PARA EXPORTAR A PDF
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

      // Uso de _webHostEnvironment. Asegúrate de que esté correctamente inicializado.
      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new UsuariosSistemaPdfDocument(usuariosParaPdf, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Lista_Usuarios_Sistema_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }
  }
}
