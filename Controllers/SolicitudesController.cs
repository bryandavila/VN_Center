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
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment
using System.IO;                   // Para Path
using QuestPDF.Fluent;             // Para QuestPDF
using VN_Center.Documents;         // Para los documentos PDF
using Microsoft.Extensions.Logging; // Para ILogger

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<SolicitudesController> _logger;

    public SolicitudesController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        IWebHostEnvironment webHostEnvironment,
        ILogger<SolicitudesController> logger)
    {
      _context = context;
      _userManager = userManager;
      _webHostEnvironment = webHostEnvironment;
      _logger = logger;
    }

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

    // ... (Acciones Details GET, Create GET/POST, Edit GET/POST, Delete GET/POST existentes) ...
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
      var model = new Solicitudes();
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

    // ACCIÓN PARA EXPORTAR DETALLE DE SOLICITUD A PDF
    public async Task<IActionResult> ExportDetailToPdf(int id)
    {
      _logger.LogInformation("Iniciando ExportDetailToPdf para Solicitud ID: {SolicitudID}", id);
      try
      {
        var solicitud = await _context.Solicitudes
            .Include(s => s.NivelesIdioma)
            .Include(s => s.FuentesConocimiento)
            .FirstOrDefaultAsync(m => m.SolicitudID == id);

        if (solicitud == null)
        {
          _logger.LogWarning("Solicitud con ID: {SolicitudID} no encontrada para exportar.", id);
          return NotFound();
        }

        if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
          _logger.LogWarning("Usuario no autorizado intentó exportar detalles de Solicitud ID: {SolicitudID}", id);
          TempData["ErrorMessage"] = "No tiene permiso para exportar los detalles de esta solicitud.";
          return RedirectToAction(nameof(Index));
        }

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

        var document = new SolicitudDetailPdfDocument(solicitud, logoPath);
        _logger.LogInformation("Documento PDF de detalle de solicitud creado, generando bytes...");
        byte[] pdfBytes = document.GeneratePdf();
        _logger.LogInformation("Bytes del PDF de detalle de solicitud generados. Longitud: {Length}", pdfBytes.Length);

        string nombreArchivo = $"Solicitud_{solicitud.Nombres?.Replace(" ", "") ?? ""}_{solicitud.Apellidos?.Replace(" ", "") ?? ""}_ID{solicitud.SolicitudID}.pdf";
        return File(pdfBytes, "application/pdf", nombreArchivo);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error durante la generación del PDF de detalle para Solicitud ID: {SolicitudID}", id);
        TempData["ErrorMessage"] = $"Ocurrió un error al generar el PDF: {ex.Message}. Revise los logs para más detalles.";
        return RedirectToAction(nameof(Details), new { id = id });
      }
    }

    // --- NUEVA ACCIÓN PARA EXPORTAR LISTA DE SOLICITUDES A PDF ---
    public async Task<IActionResult> ExportListToPdf()
    {
      _logger.LogInformation("Iniciando ExportListToPdf para Solicitudes.");
      try
      {
        IQueryable<Solicitudes> query = _context.Solicitudes
                                            .Include(s => s.FuentesConocimiento)
                                            .Include(s => s.NivelesIdioma)
                                            .OrderByDescending(s => s.FechaEnvioSolicitud);

        string tituloReporte = "Lista de Todas las Solicitudes";
        if (!User.IsInRole("Administrador"))
        {
          var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          if (!string.IsNullOrEmpty(userId))
          {
            query = query.Where(s => s.UsuarioCreadorId == userId);
            tituloReporte = "Lista de Mis Solicitudes";
          }
          else
          {
            query = query.Where(s => false);
            tituloReporte = "Lista de Solicitudes (Sin Acceso)";
          }
        }

        var solicitudes = await query.ToListAsync();
        _logger.LogInformation("Datos de solicitudes preparados para PDF. Total: {Count}", solicitudes.Count);

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");

        var document = new SolicitudesListPdfDocument(solicitudes, logoPath, tituloReporte);
        _logger.LogInformation("Documento PDF de lista de solicitudes creado, generando bytes...");
        byte[] pdfBytes = document.GeneratePdf();
        _logger.LogInformation("Bytes del PDF de lista de solicitudes generados. Longitud: {Length}", pdfBytes.Length);

        return File(pdfBytes, "application/pdf", $"Lista_Solicitudes_{DateTime.Now:yyyyMMddHHmmss}.pdf");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error durante la generación del PDF de lista de Solicitudes.");
        TempData["ErrorMessage"] = $"Ocurrió un error al generar el PDF: {ex.Message}. Revise los logs para más detalles.";
        return RedirectToAction(nameof(Index));
      }
    }
  }
}
