using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using BCryptNet = BCrypt.Net.BCrypt;

namespace VN_Center.Controllers
{
  public class UsuariosSistemaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public UsuariosSistemaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: UsuariosSistema
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.UsuariosSistema.Include(u => u.RolesSistema);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: UsuariosSistema/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var usuariosSistema = await _context.UsuariosSistema
          .Include(u => u.RolesSistema)
          .FirstOrDefaultAsync(m => m.UsuarioID == id);
      if (usuariosSistema == null)
      {
        return NotFound();
      }

      return View(usuariosSistema);
    }

    private void PopulateRolesDropDownList(object? selectedRole = null)
    {
      var rolesQuery = from r in _context.RolesSistema
                       orderby r.NombreRol
                       select r;
      ViewData["RolUsuarioID"] = new SelectList(rolesQuery.AsNoTracking(), "RolUsuarioID", "NombreRol", selectedRole);
    }

    // GET: UsuariosSistema/Create
    public IActionResult Create()
    {
      PopulateRolesDropDownList();
      return View();
    }

    // POST: UsuariosSistema/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Cambiado: Se quita HashContrasena del Bind y se añade string Password
    public async Task<IActionResult> Create(
        [Bind("UsuarioID,NombreUsuario,Nombres,Apellidos,Email,RolUsuarioID,Activo")] UsuariosSistema usuariosSistema,
        string Password) // Recibimos la contraseña en texto plano desde el formulario
    {
      ModelState.Remove("RolesSistema");
      ModelState.Remove("ProgramasProyectosONGResponsable");
      ModelState.Remove("SolicitudesInformacionGeneralAsignadas");
      ModelState.Remove("HashContrasena"); // Ya no lo enlazamos directamente

      if (string.IsNullOrWhiteSpace(Password))
      {
        ModelState.AddModelError("Password", "La contraseña es obligatoria.");
      }

      if (ModelState.IsValid)
      {
        // Hashear la contraseña antes de guardarla
        usuariosSistema.HashContrasena = BCryptNet.HashPassword(Password);

        // FechaUltimoAcceso se puede dejar nula al crear
        usuariosSistema.FechaUltimoAcceso = null;

        _context.Add(usuariosSistema);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Usuario del sistema creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateRolesDropDownList(usuariosSistema.RolUsuarioID);
      return View(usuariosSistema);
    }

    // GET: UsuariosSistema/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var usuariosSistema = await _context.UsuariosSistema.FindAsync(id);
      if (usuariosSistema == null)
      {
        return NotFound();
      }
      PopulateRolesDropDownList(usuariosSistema.RolUsuarioID);
      // No pasamos el HashContrasena a la vista para edición
      usuariosSistema.HashContrasena = ""; // Opcional: limpiar para que no se muestre si por error se renderiza
      return View(usuariosSistema);
    }

    // POST: UsuariosSistema/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Cambiado: Se quita HashContrasena del Bind y se añade string NewPassword
    public async Task<IActionResult> Edit(int id,
        [Bind("UsuarioID,NombreUsuario,Nombres,Apellidos,Email,RolUsuarioID,Activo,FechaUltimoAcceso")] UsuariosSistema usuariosSistema,
        string? NewPassword) // Recibimos la nueva contraseña (opcional)
    {
      if (id != usuariosSistema.UsuarioID)
      {
        return NotFound();
      }

      ModelState.Remove("RolesSistema");
      ModelState.Remove("ProgramasProyectosONGResponsable");
      ModelState.Remove("SolicitudesInformacionGeneralAsignadas");
      ModelState.Remove("HashContrasena"); // Ya no lo enlazamos directamente

      if (ModelState.IsValid)
      {
        try
        {
          // Obtener la entidad actual de la base de datos para no perder el hash si no se cambia
          var userToUpdate = await _context.UsuariosSistema.FirstOrDefaultAsync(u => u.UsuarioID == id);
          if (userToUpdate == null)
          {
            return NotFound();
          }

          // Actualizar las propiedades enlazadas
          userToUpdate.NombreUsuario = usuariosSistema.NombreUsuario; // Aunque usualmente no se cambia
          userToUpdate.Nombres = usuariosSistema.Nombres;
          userToUpdate.Apellidos = usuariosSistema.Apellidos;
          userToUpdate.Email = usuariosSistema.Email;
          userToUpdate.RolUsuarioID = usuariosSistema.RolUsuarioID;
          userToUpdate.Activo = usuariosSistema.Activo;
          userToUpdate.FechaUltimoAcceso = usuariosSistema.FechaUltimoAcceso; // Si se permite editar

          // Hashear y actualizar la contraseña SOLO si se proporcionó una nueva
          if (!string.IsNullOrWhiteSpace(NewPassword))
          {
            userToUpdate.HashContrasena = BCryptNet.HashPassword(NewPassword);
          }
          // Si NewPassword está vacío, userToUpdate.HashContrasena (el hash original) no se modifica.

          _context.Update(userToUpdate);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Usuario del sistema actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!UsuariosSistemaExists(usuariosSistema.UsuarioID))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      PopulateRolesDropDownList(usuariosSistema.RolUsuarioID);
      return View(usuariosSistema);
    }

    // GET: UsuariosSistema/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var usuariosSistema = await _context.UsuariosSistema
          .Include(u => u.RolesSistema)
          .FirstOrDefaultAsync(m => m.UsuarioID == id);
      if (usuariosSistema == null)
      {
        return NotFound();
      }

      return View(usuariosSistema);
    }

    // POST: UsuariosSistema/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var usuariosSistema = await _context.UsuariosSistema.FindAsync(id);
      if (usuariosSistema != null)
      {
        _context.UsuariosSistema.Remove(usuariosSistema);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Usuario del sistema eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool UsuariosSistemaExists(int id)
    {
      return _context.UsuariosSistema.Any(e => e.UsuarioID == id);
    }
  }
}
