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
using Microsoft.AspNetCore.Identity; // Necesario para UserManager
using System.Security.Claims;      // Necesario para ClaimTypes

namespace VN_Center.Controllers
{
  [Authorize]
  public class SolicitudesController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager; // Inyectar UserManager

    public SolicitudesController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager) // Modificar constructor
    {
      _context = context;
      _userManager = userManager; // Asignar UserManager
    }

    // GET: Solicitudes
    public async Task<IActionResult> Index()
    {
      IQueryable<Solicitudes> query = _context.Solicitudes
                                          .Include(s => s.FuentesConocimiento)
                                          .Include(s => s.NivelesIdioma)
                                          .OrderByDescending(s => s.FechaEnvioSolicitud);

      // Si el usuario NO es Administrador, filtrar por su UsuarioCreadorId
      if (!User.IsInRole("Administrador"))
      {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtener ID del usuario actual
        if (!string.IsNullOrEmpty(userId))
        {
          query = query.Where(s => s.UsuarioCreadorId == userId);
        }
        else
        {
          // Si no se puede obtener el ID del usuario (raro si está autenticado),
          // no mostrar ninguna solicitud para evitar fugas de datos.
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
          .AsQueryable(); // Empezar como IQueryable para añadir filtro condicional

      var solicitud = await query.FirstOrDefaultAsync(m => m.SolicitudID == id);

      if (solicitud == null)
      {
        return NotFound();
      }

      // Verificar permisos: solo admin o el creador pueden ver detalles
      if (!User.IsInRole("Administrador") && solicitud.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para ver esta solicitud.";
        return RedirectToAction(nameof(Index)); // O a una página de acceso denegado
      }

      return View(solicitud);
    }

    // GET: Solicitudes/Create
    public IActionResult Create()
    {
      ViewData["FuenteConocimientoID"] = new SelectList(_context.FuentesConocimiento, "FuenteConocimientoID", "NombreFuente");
      ViewData["NivelIdiomaEspañolID"] = new SelectList(_context.NivelesIdioma, "NivelIdiomaID", "NombreNivel");
      // Inicializar el modelo con valores por defecto si es necesario
      var model = new Solicitudes
      {
        FechaEnvioSolicitud = DateTime.UtcNow,
        EstadoSolicitud = "Recibida" // Estado por defecto
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
      // ModelState.Remove("UsuarioCreador"); // Si añades la propiedad de navegación UsuarioCreador

      if (ModelState.IsValid)
      {
        // Asignar el ID del usuario actualmente logueado
        solicitudes.UsuarioCreadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        solicitudes.FechaEnvioSolicitud = DateTime.UtcNow; // Asegurar fecha de envío actual
        if (string.IsNullOrWhiteSpace(solicitudes.EstadoSolicitud)) // Doble check por si el bind no lo toma
        {
          solicitudes.EstadoSolicitud = "Recibida";
        }

        _context.Add(solicitudes);
        await _context.SaveChangesAsync();

        // Aquí podrías añadir una llamada al servicio de auditoría si lo deseas
        // await _auditoriaService.RegistrarEventoAuditoriaAsync(...);

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

      var solicitud = await _context.Solicitudes.FindAsync(id); // FindAsync no permite .Include

      if (solicitud == null)
      {
        return NotFound();
      }

      // Verificar permisos: solo admin o el creador pueden editar
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

      // Obtener la entidad original para verificar el UsuarioCreadorId y otros datos que no deben cambiar
      var solicitudOriginal = await _context.Solicitudes.AsNoTracking().FirstOrDefaultAsync(s => s.SolicitudID == id);
      if (solicitudOriginal == null)
      {
        return NotFound();
      }

      // Verificar permisos: solo admin o el creador pueden editar
      if (!User.IsInRole("Administrador") && solicitudOriginal.UsuarioCreadorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
      {
        TempData["ErrorMessage"] = "No tiene permiso para editar esta solicitud.";
        return RedirectToAction(nameof(Index));
      }

      // Asegurarse de que UsuarioCreadorId y FechaEnvioSolicitud no se modifiquen si no es intencional
      solicitudModificada.UsuarioCreadorId = solicitudOriginal.UsuarioCreadorId;
      solicitudModificada.FechaEnvioSolicitud = solicitudOriginal.FechaEnvioSolicitud; // La fecha de envío no debería cambiar en una edición

      ModelState.Remove("NivelesIdioma");
      ModelState.Remove("FuentesConocimiento");
      ModelState.Remove("SolicitudCamposInteres");
      ModelState.Remove("ParticipacionesActivas");
      // ModelState.Remove("UsuarioCreador");

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

      // Verificar permisos: solo admin o el creador pueden ver la página de confirmación de borrado
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
        // Verificar permisos antes de eliminar
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
  }
}
