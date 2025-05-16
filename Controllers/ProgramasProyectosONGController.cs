// VN_Center/Controllers/ProgramasProyectosONGController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VN_Center.Controllers
{
  [Authorize] // Todos los usuarios autenticados pueden acceder al controlador en general
  public class ProgramasProyectosONGController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public ProgramasProyectosONGController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: ProgramasProyectosONG
    // Todos los usuarios autenticados pueden ver la lista de todos los programas
    public async Task<IActionResult> Index()
    {
      var query = _context.ProgramasProyectosONG
          .Include(p => p.ResponsablePrincipalONG)
          .OrderBy(p => p.NombreProgramaProyecto);

      // Ya no se filtra por UsuarioCreadorId para usuarios no administradores
      return View(await query.ToListAsync());
    }

    // GET: ProgramasProyectosONG/Details/5
    // Todos los usuarios autenticados pueden ver los detalles de cualquier programa
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programaProyecto = await _context.ProgramasProyectosONG
          .Include(p => p.ResponsablePrincipalONG)
          // Si necesitas más Includes para la vista de detalles (ej. comunidades, grupos vinculados), añádelos aquí.
          // .Include(p => p.ProgramaProyectoComunidades).ThenInclude(pc => pc.Comunidad) 
          // .Include(p => p.ProgramaProyectoGrupos).ThenInclude(pg => pg.GrupoComunitario)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == id);

      if (programaProyecto == null)
      {
        return NotFound();
      }
      // Ya no se verifica UsuarioCreadorId para la visualización de detalles
      return View(programaProyecto);
    }

    private async Task PopulateResponsablesDropDownListAsync(object? selectedResponsable = null)
    {
      var responsablesQuery = await _userManager.Users
                                  .OrderBy(u => u.Nombres)
                                  .ThenBy(u => u.Apellidos)
                                  .Select(u => new
                                  {
                                    u.Id,
                                    NombreCompleto = u.Nombres + " " + u.Apellidos + " (" + u.UserName + ")"
                                  })
                                  .ToListAsync();
      ViewData["ResponsablePrincipalONGUsuarioID"] = new SelectList(responsablesQuery, "Id", "NombreCompleto", selectedResponsable);
    }

    // GET: ProgramasProyectosONG/Create
    [Authorize(Roles = "Administrador")] // Solo Administradores pueden acceder a esta acción
    public async Task<IActionResult> Create()
    {
      await PopulateResponsablesDropDownListAsync();
      return View();
    }

    // POST: ProgramasProyectosONG/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores pueden ejecutar esta acción
    public async Task<IActionResult> Create([Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programasProyectosONG)
    {
      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");
      ModelState.Remove("UsuarioCreadorId"); // Aunque no se bindea, es bueno removerlo si no viene del form.

      if (ModelState.IsValid)
      {
        // Asignar el UsuarioCreadorId, útil para auditoría y saber quién lo creó
        programasProyectosONG.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _context.Add(programasProyectosONG);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateResponsablesDropDownListAsync(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
      return View(programasProyectosONG);
    }

    // GET: ProgramasProyectosONG/Edit/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programaProyecto = await _context.ProgramasProyectosONG.FindAsync(id);
      if (programaProyecto == null)
      {
        return NotFound();
      }
      // No necesitamos la verificación de UsuarioCreadorId aquí porque ya está protegido por rol.
      await PopulateResponsablesDropDownListAsync(programaProyecto.ResponsablePrincipalONGUsuarioID);
      return View(programaProyecto);
    }

    // POST: ProgramasProyectosONG/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Edit(int id, [Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programaModificado)
    {
      if (id != programaModificado.ProgramaProyectoID)
      {
        return NotFound();
      }

      // Obtener el UsuarioCreadorId original para preservarlo
      var programaOriginal = await _context.ProgramasProyectosONG.AsNoTracking().FirstOrDefaultAsync(p => p.ProgramaProyectoID == id);
      if (programaOriginal == null)
      {
        return NotFound();
      }
      programaModificado.UsuarioCreadorId = programaOriginal.UsuarioCreadorId; // Preservar el creador original

      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(programaModificado);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Programa/Proyecto actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ProgramasProyectosONGExists(programaModificado.ProgramaProyectoID))
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
      await PopulateResponsablesDropDownListAsync(programaModificado.ResponsablePrincipalONGUsuarioID);
      return View(programaModificado);
    }

    // GET: ProgramasProyectosONG/Delete/5
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programaProyecto = await _context.ProgramasProyectosONG
          .Include(p => p.ResponsablePrincipalONG)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == id);
      if (programaProyecto == null)
      {
        return NotFound();
      }
      // No necesitamos la verificación de UsuarioCreadorId aquí.
      return View(programaProyecto);
    }

    // POST: ProgramasProyectosONG/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")] // Solo Administradores
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var programaProyecto = await _context.ProgramasProyectosONG.FindAsync(id);
      if (programaProyecto != null)
      {
        // No necesitamos la verificación de UsuarioCreadorId aquí.
        _context.ProgramasProyectosONG.Remove(programaProyecto);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto eliminado exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "El programa/proyecto no fue encontrado.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool ProgramasProyectosONGExists(int id)
    {
      return _context.ProgramasProyectosONG.Any(e => e.ProgramaProyectoID == id);
    }
  }
}
