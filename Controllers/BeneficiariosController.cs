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
  public class BeneficiariosController : Controller
  {
    private readonly VNCenterDbContext _context;

    public BeneficiariosController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: Beneficiarios
    public async Task<IActionResult> Index()
    {
      var vNCenterDbContext = _context.Beneficiarios.Include(b => b.Comunidad);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: Beneficiarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null)
      {
        return NotFound();
      }

      return View(beneficiarios);
    }

    // Método privado para poblar el dropdown de Comunidades
    private void PopulateComunidadesDropDownList(object? selectedComunidad = null)
    {
      var comunidadesQuery = from c in _context.Comunidades
                             orderby c.NombreComunidad
                             select c;
      ViewData["ComunidadID"] = new SelectList(comunidadesQuery.AsNoTracking(), "ComunidadID", "NombreComunidad", selectedComunidad);
    }

    // GET: Beneficiarios/Create
    public IActionResult Create()
    {
      PopulateComunidadesDropDownList();
      return View();
    }

    // POST: Beneficiarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarios)
    {
      ModelState.Remove("Comunidad"); // Para propiedades de navegación
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");


      if (ModelState.IsValid)
      {
        _context.Add(beneficiarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario creado exitosamente.";
        return RedirectToAction(nameof(Index));
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    // GET: Beneficiarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var beneficiarios = await _context.Beneficiarios.FindAsync(id);
      if (beneficiarios == null)
      {
        return NotFound();
      }
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    // POST: Beneficiarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BeneficiarioID,FechaRegistroBeneficiario,ComunidadID,Nombres,Apellidos,RangoEdad,Genero,PaisOrigen,OtroPaisOrigen,EstadoMigratorio,OtroEstadoMigratorio,NumeroPersonasHogar,ViviendaAlquiladaOPropia,MiembrosHogarEmpleados,EstaEmpleadoPersonalmente,TipoSituacionLaboral,OtroTipoSituacionLaboral,TipoTrabajoRealizadoSiEmpleado,OtroTipoTrabajoRealizado,EstadoCivil,TiempoEnCostaRicaSiMigrante,TiempoViviendoEnComunidadActual,IngresosSuficientesNecesidades,NivelEducacionCompletado,OtroNivelEducacion,InscritoProgramaEducacionCapacitacionActual,NinosHogarAsistenEscuela,BarrerasAsistenciaEscolarNinos,OtroBarrerasAsistenciaEscolar,PercepcionAccesoIgualOportunidadesLaboralesMujeres,DisponibilidadServiciosMujeresVictimasViolencia,DisponibilidadServiciosSaludMujer,DisponibilidadServiciosApoyoAdultosMayores,AccesibilidadServiciosTransporteComunidad,AccesoComputadora,AccesoInternet")] Beneficiarios beneficiarios)
    {
      if (id != beneficiarios.BeneficiarioID)
      {
        return NotFound();
      }

      ModelState.Remove("Comunidad");
      ModelState.Remove("BeneficiarioAsistenciaRecibida");
      ModelState.Remove("BeneficiarioGrupos");
      ModelState.Remove("BeneficiariosProgramasProyectos");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(beneficiarios);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Beneficiario actualizado exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BeneficiariosExists(beneficiarios.BeneficiarioID))
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
      PopulateComunidadesDropDownList(beneficiarios.ComunidadID);
      return View(beneficiarios);
    }

    // GET: Beneficiarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var beneficiarios = await _context.Beneficiarios
          .Include(b => b.Comunidad)
          .FirstOrDefaultAsync(m => m.BeneficiarioID == id);
      if (beneficiarios == null)
      {
        return NotFound();
      }

      return View(beneficiarios);
    }

    // POST: Beneficiarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var beneficiarios = await _context.Beneficiarios.FindAsync(id);
      if (beneficiarios != null)
      {
        _context.Beneficiarios.Remove(beneficiarios);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Beneficiario eliminado exitosamente.";
      }

      return RedirectToAction(nameof(Index));
    }

    private bool BeneficiariosExists(int id)
    {
      return _context.Beneficiarios.Any(e => e.BeneficiarioID == id);
    }
  }
}
