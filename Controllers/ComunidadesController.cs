using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace VN_Center.Controllers
{
  [Authorize] // Todos los usuarios autenticados pueden acceder al controlador en general
  public class ComunidadesController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public ComunidadesController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: Comunidades
    // Todos los usuarios autenticados pueden ver la lista de todas las comunidades
    public async Task<IActionResult> Index()
    {
      // No se filtra por UsuarioCreadorId, todos ven todas las comunidades
      return View(await _context.Comunidades.OrderBy(c => c.NombreComunidad).ToListAsync());
    }

    // GET: Comunidades/Details/5
    // Todos los usuarios autenticados pueden ver los detalles de cualquier comunidad
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var comunidad = await _context.Comunidades
          .FirstOrDefaultAsync(m => m.ComunidadID == id);
      if (comunidad == null)
      {
        return NotFound();
      }
      // No se verifica UsuarioCreadorId para la visualización de detalles
      return View(comunidad);
    }

    // GET: Comunidades/Create
    [Authorize(Roles = "Administrador")] // Solo Administradores pueden acceder a esta acción
    public IActionResult Create()
    {
      return View();
    }

    // POST: Comunidades/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores pueden ejecutar esta acción
    public async Task<IActionResult> Create([Bind("ComunidadID,NombreComunidad,UbicacionDetallada,NotasComunidad")] Comunidades comunidad)
    {
      // Remover propiedades de navegación si las tuvieras en el Bind y no se setean desde el form.
      // ModelState.Remove("Beneficiarios"); 
      // ModelState.Remove("GruposComunitarios");
      // ModelState.Remove("ProgramaProyectoComunidades");
      // ModelState.Remove("UsuarioCreador"); // Si añades la propiedad de navegación

      if (ModelState.IsValid)
      {
        comunidad.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _context.Add(comunidad);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Comunidad creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      return View(comunidad);
    }

    // GET: Comunidades/Edit/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var comunidad = await _context.Comunidades.FindAsync(id);
      if (comunidad == null)
      {
        return NotFound();
      }
      // No es necesario verificar UsuarioCreadorId aquí debido a [Authorize(Roles = "Administrador")]
      return View(comunidad);
    }

    // POST: Comunidades/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int id, [Bind("ComunidadID,NombreComunidad,UbicacionDetallada,NotasComunidad")] Comunidades comunidadModificada)
    {
      if (id != comunidadModificada.ComunidadID)
      {
        return NotFound();
      }

      var comunidadOriginal = await _context.Comunidades.AsNoTracking().FirstOrDefaultAsync(c => c.ComunidadID == id);
      if (comunidadOriginal == null)
      {
        return NotFound();
      }
      // Preservar el UsuarioCreadorId original
      comunidadModificada.UsuarioCreadorId = comunidadOriginal.UsuarioCreadorId;

      // ModelState.Remove("Beneficiarios");
      // ... (otros ModelState.Remove si es necesario)

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(comunidadModificada);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Comunidad actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ComunidadesExists(comunidadModificada.ComunidadID))
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
      return View(comunidadModificada);
    }

    // GET: Comunidades/Delete/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var comunidad = await _context.Comunidades
          .FirstOrDefaultAsync(m => m.ComunidadID == id);
      if (comunidad == null)
      {
        return NotFound();
      }
      // No es necesario verificar UsuarioCreadorId aquí.
      return View(comunidad);
    }

    // POST: Comunidades/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var comunidad = await _context.Comunidades.FindAsync(id);
      if (comunidad != null)
      {
        // No es necesario verificar UsuarioCreadorId aquí.
        _context.Comunidades.Remove(comunidad);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Comunidad eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "La comunidad no fue encontrada.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool ComunidadesExists(int id)
    {
      return _context.Comunidades.Any(e => e.ComunidadID == id);
    }
  }
}
