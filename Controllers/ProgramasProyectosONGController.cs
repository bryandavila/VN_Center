using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity; // Para UserManager

namespace VN_Center.Controllers
{
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
    public async Task<IActionResult> Index()
    {
      // Usar la propiedad de navegación correcta: ResponsablePrincipalONG
      var vNCenterDbContext = _context.ProgramasProyectosONG
          .Include(p => p.ResponsablePrincipalONG);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: ProgramasProyectosONG/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programasProyectosONG = await _context.ProgramasProyectosONG
          // Usar la propiedad de navegación correcta: ResponsablePrincipalONG
          .Include(p => p.ResponsablePrincipalONG)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == id);
      if (programasProyectosONG == null)
      {
        return NotFound();
      }

      return View(programasProyectosONG);
    }

    // Método para poblar el dropdown de Usuarios Responsables
    private async Task PopulateResponsablesDropDownListAsync(object? selectedResponsable = null)
    {
      var responsablesQuery = await _userManager.Users
                                  .OrderBy(u => u.Nombres)
                                  .ThenBy(u => u.Apellidos)
                                  .Select(u => new
                                  {
                                    u.Id, // El valor del dropdown será el Id del usuario
                                    NombreCompleto = u.Nombres + " " + u.Apellidos + " (" + u.UserName + ")"
                                  })
                                  .ToListAsync();
      // El ViewData debe coincidir con el nombre de la FK en la entidad ProgramasProyectosONG
      ViewData["ResponsablePrincipalONGUsuarioID"] = new SelectList(responsablesQuery, "Id", "NombreCompleto", selectedResponsable);
    }


    // GET: ProgramasProyectosONG/Create
    public async Task<IActionResult> Create()
    {
      await PopulateResponsablesDropDownListAsync();
      return View();
    }

    // POST: ProgramasProyectosONG/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Asegúrate que el Bind usa la FK correcta: ResponsablePrincipalONGUsuarioID
    public async Task<IActionResult> Create([Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programasProyectosONG)
    {
      // La propiedad de navegación se llama ResponsablePrincipalONG
      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");
      ModelState.Remove("EvaluacionesPrograma"); // Añadido basado en tu entidad original

      if (ModelState.IsValid)
      {
        _context.Add(programasProyectosONG);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      await PopulateResponsablesDropDownListAsync(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
      return View(programasProyectosONG);
    }

    // GET: ProgramasProyectosONG/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programasProyectosONG = await _context.ProgramasProyectosONG.FindAsync(id);
      if (programasProyectosONG == null)
      {
        return NotFound();
      }
      await PopulateResponsablesDropDownListAsync(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
      return View(programasProyectosONG);
    }

    // POST: ProgramasProyectosONG/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Asegúrate que el Bind usa la FK correcta: ResponsablePrincipalONGUsuarioID
    public async Task<IActionResult> Edit(int id, [Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programasProyectosONG)
    {
      if (id != programasProyectosONG.ProgramaProyectoID)
      {
        return NotFound();
      }

      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");
      ModelState.Remove("EvaluacionesPrograma"); // Añadido

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(programasProyectosONG);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Programa/Proyecto actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ProgramasProyectosONGExists(programasProyectosONG.ProgramaProyectoID))
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
      await PopulateResponsablesDropDownListAsync(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
      return View(programasProyectosONG);
    }

    // GET: ProgramasProyectosONG/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var programasProyectosONG = await _context.ProgramasProyectosONG
          // Usar la propiedad de navegación correcta: ResponsablePrincipalONG
          .Include(p => p.ResponsablePrincipalONG)
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == id);
      if (programasProyectosONG == null)
      {
        return NotFound();
      }

      return View(programasProyectosONG);
    }

    // POST: ProgramasProyectosONG/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var programasProyectosONG = await _context.ProgramasProyectosONG.FindAsync(id);
      if (programasProyectosONG != null)
      {
        _context.ProgramasProyectosONG.Remove(programasProyectosONG);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ProgramasProyectosONGExists(int id)
    {
      return _context.ProgramasProyectosONG.Any(e => e.ProgramaProyectoID == id);
    }
  }
}
