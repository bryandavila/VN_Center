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

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesController : Controller
  {
    private readonly VNCenterDbContext _context;

    public SolicitudesController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: Solicitudes
    public async Task<IActionResult> Index()
    {
      // Incluir datos relacionados para mostrar en la vista Index
      // Esto es importante si en tu vista Index quieres mostrar, por ejemplo,
      // el nombre del Nivel de Idioma en lugar de solo el ID.
      var vNCenterDbContext = _context.Solicitudes
                                      .Include(s => s.FuentesConocimiento)
                                      .Include(s => s.NivelesIdioma);
      return View(await vNCenterDbContext.ToListAsync());
    }

    // GET: Solicitudes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudes = await _context.Solicitudes
          .Include(s => s.FuentesConocimiento) // Incluye la entidad relacionada
          .Include(s => s.NivelesIdioma)     // Incluye la entidad relacionada
          .FirstOrDefaultAsync(m => m.SolicitudID == id);

      if (solicitudes == null)
      {
        return NotFound();
      }

      return View(solicitudes);
    }

    // GET: Solicitudes/Create
    public IActionResult Create()
    {
      // Poblar ViewBag/ViewData para los dropdowns
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente");
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel");
      return View();
    }

    // POST: Solicitudes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudID,Nombres,Apellidos,Email,Telefono,PermiteContactoWhatsApp,Direccion,FechaNacimiento,TipoSolicitud,FechaEnvioSolicitud,PasaporteValidoSeisMeses,FechaExpiracionPasaporte,ProfesionOcupacion,NivelIdiomaEspañolID,OtrosIdiomasHablados,ExperienciaPreviaSVL,ExperienciaLaboralRelevante,HabilidadesRelevantes,FechaInicioPreferida,DuracionEstancia,DuracionEstanciaOtro,MotivacionInteresCR,DescripcionSalidaZonaConfort,InformacionAdicionalPersonal,FuenteConocimientoID,FuenteConocimientoOtro,PathCV,PathCartaRecomendacion,EstadoSolicitud,NombreUniversidad,CarreraAreaEstudio,FechaGraduacionEsperada,PreferenciasAlojamientoFamilia,EnsayoRelacionEstudiosIntereses,EnsayoHabilidadesConocimientosAdquirir,EnsayoContribucionAprendizajeComunidad,EnsayoExperienciasTransculturalesPrevias,NombreContactoEmergencia,TelefonoContactoEmergencia,EmailContactoEmergencia,RelacionContactoEmergencia,AniosEntrenamientoFormalEsp,ComodidadHabilidadesEsp,InfoPersonalFamiliaHobbies,AlergiasRestriccionesDieteticas,SolicitudesEspecialesAlojamiento")] Solicitudes solicitudes)
    {
      // Remover validación para propiedades de navegación si no se están seteando directamente
      // y EF Core las manejará a través de las FK IDs.
      ModelState.Remove("NivelesIdioma");
      ModelState.Remove("FuentesConocimiento");
      ModelState.Remove("SolicitudCamposInteres");
      ModelState.Remove("ParticipacionesActivas");

      if (ModelState.IsValid)
      {
        _context.Add(solicitudes);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud creada exitosamente."; // Mensaje de éxito
        return RedirectToAction(nameof(Index));
      }
      // Si el modelo no es válido, volver a poblar los ViewBag/ViewData para los dropdowns
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente", solicitudes.FuenteConocimientoID);
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel", solicitudes.NivelIdiomaEspañolID);
      return View(solicitudes);
    }

    // GET: Solicitudes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudes = await _context.Solicitudes.FindAsync(id);
      if (solicitudes == null)
      {
        return NotFound();
      }
      // Poblar ViewBag/ViewData para los dropdowns en la vista de edición
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente", solicitudes.FuenteConocimientoID);
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel", solicitudes.NivelIdiomaEspañolID);
      return View(solicitudes);
    }

    // POST: Solicitudes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudID,Nombres,Apellidos,Email,Telefono,PermiteContactoWhatsApp,Direccion,FechaNacimiento,TipoSolicitud,FechaEnvioSolicitud,PasaporteValidoSeisMeses,FechaExpiracionPasaporte,ProfesionOcupacion,NivelIdiomaEspañolID,OtrosIdiomasHablados,ExperienciaPreviaSVL,ExperienciaLaboralRelevante,HabilidadesRelevantes,FechaInicioPreferida,DuracionEstancia,DuracionEstanciaOtro,MotivacionInteresCR,DescripcionSalidaZonaConfort,InformacionAdicionalPersonal,FuenteConocimientoID,FuenteConocimientoOtro,PathCV,PathCartaRecomendacion,EstadoSolicitud,NombreUniversidad,CarreraAreaEstudio,FechaGraduacionEsperada,PreferenciasAlojamientoFamilia,EnsayoRelacionEstudiosIntereses,EnsayoHabilidadesConocimientosAdquirir,EnsayoContribucionAprendizajeComunidad,EnsayoExperienciasTransculturalesPrevias,NombreContactoEmergencia,TelefonoContactoEmergencia,EmailContactoEmergencia,RelacionContactoEmergencia,AniosEntrenamientoFormalEsp,ComodidadHabilidadesEsp,InfoPersonalFamiliaHobbies,AlergiasRestriccionesDieteticas,SolicitudesEspecialesAlojamiento")] Solicitudes solicitudes)
    {
      if (id != solicitudes.SolicitudID)
      {
        return NotFound();
      }

      // Remover validación para propiedades de navegación
      ModelState.Remove("NivelesIdioma");
      ModelState.Remove("FuentesConocimiento");
      ModelState.Remove("SolicitudCamposInteres");
      ModelState.Remove("ParticipacionesActivas");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(solicitudes);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Solicitud actualizada exitosamente."; // Mensaje de éxito
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SolicitudesExists(solicitudes.SolicitudID))
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
      // Si el modelo no es válido, volver a poblar los ViewBag/ViewData para los dropdowns
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente", solicitudes.FuenteConocimientoID);
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel", solicitudes.NivelIdiomaEspañolID);
      return View(solicitudes);
    }

    // GET: Solicitudes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var solicitudes = await _context.Solicitudes
          .Include(s => s.FuentesConocimiento) // Incluir para mostrar en la vista de confirmación
          .Include(s => s.NivelesIdioma)     // Incluir para mostrar en la vista de confirmación
          .FirstOrDefaultAsync(m => m.SolicitudID == id);
      if (solicitudes == null)
      {
        return NotFound();
      }

      return View(solicitudes);
    }

    // POST: Solicitudes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var solicitudes = await _context.Solicitudes.FindAsync(id);
      if (solicitudes != null)
      {
        _context.Solicitudes.Remove(solicitudes);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud eliminada exitosamente."; // Mensaje de éxito
      }

      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudesExists(int id)
    {
      return _context.Solicitudes.Any(e => e.SolicitudID == id);
    }
  }
}
