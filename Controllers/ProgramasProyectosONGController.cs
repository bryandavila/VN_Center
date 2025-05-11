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
  public class ProgramasProyectosONGController : Controller
  {
    private readonly VNCenterDbContext _context;

    public ProgramasProyectosONGController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: ProgramasProyectosONG
    public async Task<IActionResult> Index()
    {
      // Incluye la entidad relacionada 'ResponsablePrincipalONG' para mostrar su nombre en la vista Index.
      var vNCenterDbContext = _context.ProgramasProyectosONG.Include(p => p.ResponsablePrincipalONG);
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
          .Include(p => p.ResponsablePrincipalONG) // Incluye la entidad relacionada
          .FirstOrDefaultAsync(m => m.ProgramaProyectoID == id);
      if (programasProyectosONG == null)
      {
        return NotFound();
      }

      return View(programasProyectosONG);
    }

    // Método privado para cargar la lista de usuarios responsables para el dropdown.
    // Esto evita la repetición de código en las acciones Create y Edit.
    private void PopulateResponsablesDropDownList(object? selectedResponsable = null)
    {
      // Crea una consulta para obtener UsuarioID y NombreCompleto (Nombres + Apellidos)
      var responsablesQuery = from u in _context.UsuariosSistema
                              orderby u.Nombres, u.Apellidos
                              select new
                              {
                                UsuarioID = u.UsuarioID,
                                NombreCompleto = u.Nombres + " " + u.Apellidos
                              };
      // Pasa la lista al ViewData para ser usada en la vista por un <select> Tag Helper.
      // "UsuarioID" es el valor de la opción, "NombreCompleto" es el texto a mostrar.
      // selectedResponsable es el valor que estará preseleccionado (útil para la vista Edit).
      ViewData["ResponsablePrincipalONGUsuarioID"] = new SelectList(responsablesQuery.AsNoTracking(), "UsuarioID", "NombreCompleto", selectedResponsable);
    }


    // GET: ProgramasProyectosONG/Create
    public IActionResult Create()
    {
      // Llama al método para poblar el dropdown de responsables.
      PopulateResponsablesDropDownList();
      return View();
    }

    // POST: ProgramasProyectosONG/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programasProyectosONG)
    {
      // Remueve la validación para las propiedades de navegación.
      // Solo nos interesa validar las propiedades escalares y las FK IDs que se enlazan desde el formulario.
      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        _context.Add(programasProyectosONG);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Programa/Proyecto creado exitosamente."; // Mensaje de éxito para mostrar en la siguiente vista.
        return RedirectToAction(nameof(Index));
      }
      // Si el modelo no es válido, vuelve a poblar el dropdown antes de retornar la vista.
      PopulateResponsablesDropDownList(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
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
      // Llama al método para poblar el dropdown de responsables, preseleccionando el actual.
      PopulateResponsablesDropDownList(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
      return View(programasProyectosONG);
    }

    // POST: ProgramasProyectosONG/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProgramaProyectoID,NombreProgramaProyecto,Descripcion,TipoIniciativa,FechaInicioEstimada,FechaFinEstimada,FechaInicioReal,FechaFinReal,EstadoProgramaProyecto,ResponsablePrincipalONGUsuarioID")] ProgramasProyectosONG programasProyectosONG)
    {
      if (id != programasProyectosONG.ProgramaProyectoID)
      {
        return NotFound();
      }

      // Remueve la validación para las propiedades de navegación.
      ModelState.Remove("ResponsablePrincipalONG");
      ModelState.Remove("ProgramaProyectoComunidades");
      ModelState.Remove("ProgramaProyectoGrupos");
      ModelState.Remove("ParticipacionesActivas");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(programasProyectosONG);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Programa/Proyecto actualizado exitosamente."; // Mensaje de éxito.
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
      // Si el modelo no es válido, vuelve a poblar el dropdown.
      PopulateResponsablesDropDownList(programasProyectosONG.ResponsablePrincipalONGUsuarioID);
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
          .Include(p => p.ResponsablePrincipalONG) // Incluye para mostrar info en la confirmación.
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
        TempData["SuccessMessage"] = "Programa/Proyecto eliminado exitosamente."; // Mensaje de éxito.
      }

      return RedirectToAction(nameof(Index));
    }

    private bool ProgramasProyectosONGExists(int id)
    {
      return _context.ProgramasProyectosONG.Any(e => e.ProgramaProyectoID == id);
    }
  }
}
