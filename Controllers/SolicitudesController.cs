// VN_Center/Controllers/SolicitudesController.cs
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
using Microsoft.AspNetCore.Hosting; // <--- AÑADIDO para IWebHostEnvironment
using System.IO;                   // <--- AÑADIDO para Path
using QuestPDF.Fluent;             // <--- AÑADIDO para QuestPDF
using VN_Center.Documents;         // <--- AÑADIDO para SolicitudDetailPdfDocument

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment; // <--- AÑADIDO

    public SolicitudesController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        IWebHostEnvironment webHostEnvironment) // <--- MODIFICADO
    {
      _context = context;
      _userManager = userManager;
      _webHostEnvironment = webHostEnvironment; // <--- AÑADIDO
    }

    // ... (Acciones Index, Details GET, Create GET/POST, Edit GET/POST, Delete GET/POST existentes) ...
    // El código de esas acciones ya incluye el filtrado por UsuarioCreadorId y los permisos.

    // GET: Solicitudes
    public async Task<IActionResult> Index()
    {
      IQueryable<Solicitudes> query = _context.Solicitudes
                                          .Include(s => s.FuentesConocimiento)
                                          .Include(s => s.NivelesIdioma)
                                          .OrderByDescending(s => s.FechaEnvioSolicitud);

      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(s => s.UsuarioCreadorId == userId);
        }
        else
        {
          query = query.Where(s => false);
        }
      }

      return View(await query.ToListAsync());
    }

    // GET: Solicitudes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var query = _context.Solicitudes
          .Include(s => s.FuentesConocimiento)
          .Include(s => s.NivelesIdioma)
          .AsQueryable();

      var solicitud = await query.FirstOrDefaultAsync(m => m.SolicitudID == id);

      if (solicitud == null)
      {
        return NotFound();
      }

      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para ver esta solicitud.";
        return RedirectToAction(nameof(Index));
      }

      return View(solicitud);
    }

    // GET: Solicitudes/Create
    public IActionResult Create()
    {
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente");
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel");
      var model = new Solicitudes
      {
        FechaEnvioSolicitud = DateTime.UtcNow,
        EstadoSolicitud = "Recibida"
      };
      return View(model);
    }

    // POST: Solicitudes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SolicitudID,Nombres,Apellidos,Email,Telefono,PermiteContactoWhatsApp,Direccion,FechaNacimiento,TipoSolicitud,PasaporteValidoSeisMeses,FechaExpiracionPasaporte,ProfesionOcupacion,NivelIdiomaEspañolID,OtrosIdiomasHablados,ExperienciaPreviaSVL,ExperienciaLaboralRelevante,HabilidadesRelevantes,FechaInicioPreferida,DuracionEstancia,DuracionEstanciaOtro,MotivacionInteresCR,DescripcionSalidaZonaConfort,InformacionAdicionalPersonal,FuenteConocimientoID,FuenteConocimientoOtro,PathCV,PathCartaRecomendacion,EstadoSolicitud,NombreUniversidad,CarreraAreaEstudio,FechaGraduacionEsperada,PreferenciasAlojamientoFamilia,EnsayoRelacionEstudiosIntereses,EnsayoHabilidadesConocimientosAdquirir,EnsayoContribucionAprendizajeComunidad,EnsayoExperienciasTransculturalesPrevias,NombreContactoEmergencia,TelefonoContactoEmergencia,EmailContactoEmergencia,RelacionContactoEmergencia,AniosEntrenamientoFormalEsp,ComodidadHabilidadesEsp,InfoPersonalFamiliaHobbies,AlergiasRestriccionesDieteticas,SolicitudesEspecialesAlojamiento")] Solicitudes solicitudes)
    {
      ModelState.Remove("NivelesIdioma");
      ModelState.Remove("FuentesConocimiento");
      ModelState.Remove("SolicitudCamposInteres");
      ModelState.Remove("ParticipacionesActivas");

      if (ModelState.IsValid)
      {
        solicitudes.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        solicitudes.FechaEnvioSolicitud = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(solicitudes.EstadoSolicitud))
        {
          solicitudes.EstadoSolicitud = "Recibida";
        }
        _context.Add(solicitudes);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud creada exitosamente.";
        return RedirectToAction(nameof(Index));
      }
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
      var solicitud = await _context.Solicitudes.FindAsync(id);
      if (solicitud == null)
      {
        return NotFound();
      }
      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para editar esta solicitud.";
        return RedirectToAction(nameof(Index));
      }
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente", solicitud.FuenteConocimientoID);
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel", solicitud.NivelIdiomaEspañolID);
      return View(solicitud);
    }

    // POST: Solicitudes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("SolicitudID,Nombres,Apellidos,Email,Telefono,PermiteContactoWhatsApp,Direccion,FechaNacimiento,TipoSolicitud,FechaEnvioSolicitud,PasaporteValidoSeisMeses,FechaExpiracionPasaporte,ProfesionOcupacion,NivelIdiomaEspañolID,OtrosIdiomasHablados,ExperienciaPreviaSVL,ExperienciaLaboralRelevante,HabilidadesRelevantes,FechaInicioPreferida,DuracionEstancia,DuracionEstanciaOtro,MotivacionInteresCR,DescripcionSalidaZonaConfort,InformacionAdicionalPersonal,FuenteConocimientoID,FuenteConocimientoOtro,PathCV,PathCartaRecomendacion,EstadoSolicitud,NombreUniversidad,CarreraAreaEstudio,FechaGraduacionEsperada,PreferenciasAlojamientoFamilia,EnsayoRelacionEstudiosIntereses,EnsayoHabilidadesConocimientosAdquirir,EnsayoContribucionAprendizajeComunidad,EnsayoExperienciasTransculturalesPrevias,NombreContactoEmergencia,TelefonoContactoEmergencia,EmailContactoEmergencia,RelacionContactoEmergencia,AniosEntrenamientoFormalEsp,ComodidadHabilidadesEsp,InfoPersonalFamiliaHobbies,AlergiasRestriccionesDieteticas,SolicitudesEspecialesAlojamiento,UsuarioCreadorId")] Solicitudes solicitudModificada)
    {
      if (id != solicitudModificada.SolicitudID)
      {
        return NotFound();
      }
      var solicitudOriginal = await _context.Solicitudes.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudID == id);
      if (solicitudOriginal == null)
      {
        return NotFound();
      }
      if (!User.IsInRole("Administrador") && solicitudOriginal.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para editar esta solicitud.";
        return RedirectToAction(nameof(Index));
      }
      solicitudModificada.UsuarioCreadorId = solicitudOriginal.UsuarioCreadorId;
      solicitudModificada.FechaEnvioSolicitud = solicitudOriginal.FechaEnvioSolicitud;
      ModelState.Remove("NivelesIdioma");
      ModelState.Remove("FuentesConocimiento");
      ModelState.Remove("SolicitudCamposInteres");
      ModelState.Remove("ParticipacionesActivas");

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(solicitudModificada);
          await _context.SaveChangesAsync();
          TempData["SuccessMessage"] = "Solicitud actualizada exitosamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SolicitudesExists(solicitudModificada.SolicitudID))
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
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente", solicitudModificada.FuenteConocimientoID);
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel", solicitudModificada.NivelIdiomaEspañolID);
      return View(solicitudModificada);
    }

    // GET: Solicitudes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var solicitud = await _context.Solicitudes
          .Include(s => s.FuentesConocimiento)
          .Include(s => s.NivelesIdioma)
          .FirstOrDefaultAsync(m => m.SolicitudID == id);
      if (solicitud == null)
      {
        return NotFound();
      }
      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para eliminar esta solicitud.";
        return RedirectToAction(nameof(Index));
      }
      return View(solicitud);
    }

    // POST: Solicitudes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var solicitud = await _context.Solicitudes.FindAsync(id);
      if (solicitud != null)
      {
        if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
          TempData["ErrorMessage"] = "No tiene permiso para eliminar esta solicitud.";
          return RedirectToAction(nameof(Index));
        }
        _context.Solicitudes.Remove(solicitud);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud eliminada exitosamente.";
      }
      else
      {
        TempData["ErrorMessage"] = "La solicitud no fue encontrada.";
      }
      return RedirectToAction(nameof(Index));
    }

    private bool SolicitudesExists(int id)
    {
      return _context.Solicitudes.Any(e => e.SolicitudID == id);
    }

    // --- NUEVA ACCIÓN PARA EXPORTAR DETALLE DE SOLICITUD A PDF ---
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      var solicitud = await _context.Solicitudes
          .Include(s => s.NivelesIdioma) // Incluir datos relacionados necesarios para el PDF
          .Include(s => s.FuentesConocimiento)
          // Incluir otras relaciones si se muestran en el PDF de detalle
          // .Include(s => s.SolicitudCamposInteres).ThenInclude(sci => sci.CampoInteresVocacional) 
          .FirstOrDefaultAsync(m => m.SolicitudID == id);

      if (solicitud == null)
      {
        return NotFound();
      }

      // Verificar permisos: solo admin o el creador pueden exportar detalles
      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para exportar los detalles de esta solicitud.";
        return RedirectToAction(nameof(Index));
      }

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

      var document = new SolicitudDetailPdfDocument(solicitud, logoPath);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"Solicitud_{solicitud.Nombres}_{solicitud.Apellidos}_ID{solicitud.SolicitudID}.pdf");
    }
  }
}
