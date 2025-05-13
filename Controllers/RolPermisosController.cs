using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity; // Necesario para RoleManager

namespace VN_Center.Controllers
{
  // TODO: Considerar añadir autorización si es necesario
  public class RolPermisosController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly RoleManager<RolesSistema> _roleManager; // Inyectar RoleManager

    public RolPermisosController(VNCenterDbContext context, RoleManager<RolesSistema> roleManager)
    {
      _context = context;
      _roleManager = roleManager; // Asignar RoleManager
    }

    // GET: RolPermisos
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.RolPermisos
                                    .Include(r => r.Permisos)
                                    .Include(r => r.RolesSistema) // RolesSistema es la entidad de Identity
                                    .OrderBy(r => r.RolesSistema.Name) // Ordenar por RolesSistema.Name
                                    .ThenBy(r => r.Permisos.NombrePermiso);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: RolPermisos/Details/rolId/permisoId
    public async Task<IActionResult> Details(int? rolId, int? permisoId)
    {
      if (rolId == null || permisoId == null)
      {
        return NotFound();
      }

      var rolPermisos = await _context.RolPermisos
          .Include(r => r.Permisos)
          .Include(r => r.RolesSistema) // RolesSistema es la entidad de Identity
          .FirstOrDefaultAsync(m => m.RolUsuarioID == rolId && m.PermisoID == permisoId);

      if (rolPermisos == null)
      {
        return NotFound();
      }

      return View(rolPermisos);
    }

    // Método actualizado para poblar el dropdown de Roles usando RoleManager
    private async Task PopulateRolesDropDownListAsync(object? selectedRole = null)
    {
      // Obtener roles desde RoleManager
      var rolesQuery = await _roleManager.Roles
                                  .OrderBy(r => r.Name) // Usar Name de IdentityRole
                                  .ToListAsync();
      // Usar "Id" como valor y "Name" como texto para el SelectList
      ViewData["RolUsuarioID"] = new SelectList(rolesQuery, "Id", "Name", selectedRole);
    }

    private async Task PopulatePermisosDropDownListAsync(object? selectedPermiso = null)
    {
      var permisosQuery = await _context.Permisos
                                   .OrderBy(p => p.NombrePermiso)
                                   .ToListAsync(); // Es bueno usar ToListAsync si se va a iterar o pasar a SelectList
      ViewData["PermisoID"] = new SelectList(permisosQuery, "PermisoID", "NombrePermiso", selectedPermiso);
    }

    // GET: RolPermisos/Create
    public async Task<IActionResult> Create()
    {
      // Usar las versiones asíncronas
      await PopulateRolesDropDownListAsync();
      await PopulatePermisosDropDownListAsync();
      return View();
    }

    // POST: RolPermisos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RolUsuarioID,PermisoID")] RolPermisos rolPermisos)
    {
      // Verificar si la combinación Rol-Permiso ya existe
      if (await _context.RolPermisos.AnyAsync(rp => rp.RolUsuarioID == rolPermisos.RolUsuarioID && rp.PermisoID == rolPermisos.PermisoID))
      {
        ModelState.AddModelError(string.Empty, "Este permiso ya está asignado a este rol.");
      }

      if (ModelState.IsValid)
      {
        _context.Add(rolPermisos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Permiso asignado al rol exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateRolesDropDownListAsync(rolPermisos.RolUsuarioID);
      await PopulatePermisosDropDownListAsync(rolPermisos.PermisoID);
      return View(rolPermisos);
    }

    // Las acciones Edit para una tabla de cruce simple (sin datos adicionales en la tabla de cruce)
    // a menudo se omiten, ya que "editar" una asignación usualmente significa eliminar la antigua y crear una nueva.
    // Si decides implementarla, asegúrate de que la lógica y la UX sean claras.
    // Por ahora, se mantienen comentadas como en tu código original.
    /*
    public async Task<IActionResult> Edit(int? rolId, int? permisoId)
    {
        // ...
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int rolId, int permisoId, [Bind("RolUsuarioID,PermisoID")] RolPermisos rolPermisos)
    {
        // ...
    }
    */

    // GET: RolPermisos/Delete/rolId/permisoId
    public async Task<IActionResult> Delete(int? rolId, int? permisoId)
    {
      if (rolId == null || permisoId == null)
      {
        return NotFound();
      }

      var rolPermisos = await _context.RolPermisos
          .Include(r => r.Permisos)
          .Include(r => r.RolesSistema) // RolesSistema es la entidad de Identity
          .FirstOrDefaultAsync(m => m.RolUsuarioID == rolId && m.PermisoID == permisoId);

      if (rolPermisos == null)
      {
        return NotFound();
      }

      return View(rolPermisos);
    }

    // POST: RolPermisos/DeleteConfirmed
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    // Los nombres de los parámetros deben coincidir con los campos del formulario/ruta (RolUsuarioID, PermisoID)
    public async Task<IActionResult> DeleteConfirmed(int RolUsuarioID, int PermisoID)
    {
      // FindAsync funciona bien con claves compuestas si están definidas correctamente en OnModelCreating
      var rolPermisos = await _context.RolPermisos.FindAsync(RolUsuarioID, PermisoID);
      if (rolPermisos != null)
      {
        _context.RolPermisos.Remove(rolPermisos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de permiso eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "No se encontró la asignación de permiso a eliminar.";
      }

      return RedirectToAction(nameof(Index));
    }

    // El método RolPermisosExists es para la clave compuesta y está bien como está.
    private bool RolPermisosExists(int rolId, int permisoId)
    {
      return _context.RolPermisos.Any(e => e.RolUsuarioID == rolId && e.PermisoID == permisoId);
    }
  }
}
