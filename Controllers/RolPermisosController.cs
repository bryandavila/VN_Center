using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;

namespace VN_Center.Controllers
{
  public class RolPermisosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public RolPermisosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: RolPermisos
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.RolPermisos
                                      .Include(r => r.Permisos)
                                      .Include(r => r.RolesSistema)
                                      .OrderBy(r => r.RolesSistema.NombreRol)
                                      .ThenBy(r => r.Permisos.NombrePermiso);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: RolPermisos/Details/5 (Details para una tabla de cruce pura puede no ser muy útil,
    // pero el scaffolder la crea. La dejaremos por ahora.)
    // Para Details, necesitamos ambos IDs. El scaffolder crea Details(int? id) que no funciona para PK compuesta.
    // Lo ajustaremos para que tome ambos IDs.
    public async Task<IActionResult> Details(int? rolId, int? permisoId)
    {
      if (rolId == null || permisoId == null)
      {
        return NotFound();
      }

      var rolPermisos = await _context.RolPermisos
          .Include(r => r.Permisos)
          .Include(r => r.RolesSistema)
          .FirstOrDefaultAsync(m => m.RolUsuarioID == rolId && m.PermisoID == permisoId);
      if (rolPermisos == null)
      {
        return NotFound();
      }

      return View(rolPermisos);
    }

    private void PopulateRolesDropDownList(object? selectedRole = null)
    {
      var rolesQuery = from r in _context.RolesSistema
                       orderby r.NombreRol
                       select r;
      ViewData["RolUsuarioID"] = new SelectList(rolesQuery.AsNoTracking(), "RolUsuarioID", "NombreRol", selectedRole);
    }

    private void PopulatePermisosDropDownList(object? selectedPermiso = null)
    {
      var permisosQuery = from p in _context.Permisos
                          orderby p.NombrePermiso
                          select p;
      ViewData["PermisoID"] = new SelectList(permisosQuery.AsNoTracking(), "PermisoID", "NombrePermiso", selectedPermiso);
    }


    // GET: RolPermisos/Create
    public IActionResult Create()
    {
      PopulateRolesDropDownList();
      PopulatePermisosDropDownList();
      return View();
    }

    // POST: RolPermisos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RolUsuarioID,PermisoID")] RolPermisos rolPermisos)
    {
      // No hay otras propiedades de navegación directas en RolPermisos que necesiten ModelState.Remove
      // ModelState.Remove("RolesSistema"); // Ya no es necesario si solo se bindean los IDs
      // ModelState.Remove("Permisos");

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
      PopulateRolesDropDownList(rolPermisos.RolUsuarioID);
      PopulatePermisosDropDownList(rolPermisos.PermisoID);
      return View(rolPermisos);
    }

    // GET: RolPermisos/Edit/5 (Edit para una PK compuesta es problemático con el scaffolder por defecto)
    // Lo ajustaremos para que tome ambos IDs. El concepto de "Editar" una asignación es usualmente eliminarla y crear una nueva
    // si se quiere cambiar el rol o el permiso. Si la tabla de cruce tuviera datos adicionales, sí se editaría.
    // Por ahora, el scaffolder genera un Edit(int? id) que no funcionará.
    // Una mejor UX sería gestionar esto desde la página de edición de un Rol (listar sus permisos y permitir añadir/quitar).
    // Por simplicidad del CRUD generado, la acción Edit aquí podría no ser muy útil o necesitar un rediseño completo.
    // Vamos a comentarla por ahora, ya que la funcionalidad principal es Crear y Eliminar asignaciones.
    /*
    public async Task<IActionResult> Edit(int? rolId, int? permisoId)
    {
        if (rolId == null || permisoId == null)
        {
            return NotFound();
        }

        var rolPermisos = await _context.RolPermisos.FindAsync(rolId, permisoId); // FindAsync funciona con PK compuesta
        if (rolPermisos == null)
        {
            return NotFound();
        }
        PopulateRolesDropDownList(rolPermisos.RolUsuarioID);
        PopulatePermisosDropDownList(rolPermisos.PermisoID);
        return View(rolPermisos);
    }
    */

    // POST: RolPermisos/Edit/5
    // Comentado por las mismas razones que el GET de Edit.
    /*
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int rolId, int permisoId, [Bind("RolUsuarioID,PermisoID")] RolPermisos rolPermisos)
    {
        if (rolId != rolPermisos.RolUsuarioID || permisoId != rolPermisos.PermisoID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(rolPermisos);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asignación de permiso actualizada exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolPermisosExists(rolPermisos.RolUsuarioID, rolPermisos.PermisoID))
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
        PopulateRolesDropDownList(rolPermisos.RolUsuarioID);
        PopulatePermisosDropDownList(rolPermisos.PermisoID);
        return View(rolPermisos);
    }
    */

    // GET: RolPermisos/Delete/5
    // Ajustado para tomar ambos IDs de la clave compuesta.
    public async Task<IActionResult> Delete(int? rolId, int? permisoId)
    {
      if (rolId == null || permisoId == null)
      {
        return NotFound();
      }

      var rolPermisos = await _context.RolPermisos
          .Include(r => r.Permisos)
          .Include(r => r.RolesSistema)
          .FirstOrDefaultAsync(m => m.RolUsuarioID == rolId && m.PermisoID == permisoId);
      if (rolPermisos == null)
      {
        return NotFound();
      }

      return View(rolPermisos);
    }

    // POST: RolPermisos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    // Ajustado para tomar ambos IDs de la clave compuesta.
    public async Task<IActionResult> DeleteConfirmed(int RolUsuarioID, int PermisoID) // Parámetros deben coincidir con los valores de la ruta/formulario
    {
      var rolPermisos = await _context.RolPermisos.FindAsync(RolUsuarioID, PermisoID);
      if (rolPermisos != null)
      {
        _context.RolPermisos.Remove(rolPermisos);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Asignación de permiso eliminada exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool RolPermisosExists(int rolId, int permisoId)
    {
      return _context.RolPermisos.Any(e => e.RolUsuarioID == rolId && e.PermisoID == permisoId);
    }
  }
}
