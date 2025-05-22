using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.ViewModels;
using System.Globalization;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Para UserManager
using System.Security.Claims;     // Para ClaimTypes

namespace VN_Center.Controllers
{
  [Authorize]
  public class DashboardsController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager; // Inyectar UserManager
    private const int NumeroItemsRecientes = 5;

    public DashboardsController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager) // Modificar constructor
    {
      _context = context;
      _userManager = userManager; // Asignar UserManager
    }

    public async Task<IActionResult> Index()
    {
      var viewModel = new DashboardViewModel();
      var currentUserId = _userManager.GetUserId(User); // Obtener ID del usuario actual

      if (User.IsInRole("Administrador"))
      {
        // --- LÓGICA PARA ADMINISTRADOR (EXISTENTE) ---
        viewModel.TotalSolicitudes = await _context.Solicitudes.CountAsync();
        viewModel.TotalBeneficiarios = await _context.Beneficiarios.CountAsync();
        viewModel.TotalProgramasActivos = await _context.ProgramasProyectosONG
                                            .CountAsync(p => p.EstadoProgramaProyecto == "En Curso" || p.EstadoProgramaProyecto == "Planificación");
        viewModel.TotalComunidades = await _context.Comunidades.CountAsync();

        var haceUnAnio = DateTime.UtcNow.AddYears(-1);
        var solicitudesPorMesData = await _context.Solicitudes
            .Where(s => s.FechaEnvioSolicitud >= haceUnAnio)
            .GroupBy(s => new { Anio = s.FechaEnvioSolicitud.Year, Mes = s.FechaEnvioSolicitud.Month })
            .Select(g => new SolicitudMensualViewModel
            {
              Anio = g.Key.Anio,
              Mes = g.Key.Mes,
              Cantidad = g.Count()
            })
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Mes)
            .ToListAsync();

        var culturaES = new CultureInfo("es-ES");
        var todosLosMeses = Enumerable.Range(0, 12)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Select(d => new { Anio = d.Year, Mes = d.Month, MesNombre = d.ToString("MMM yy", culturaES) })
            .OrderBy(x => x.Anio).ThenBy(x => x.Mes)
            .ToList();

        viewModel.SolicitudesPorMes = todosLosMeses
            .GroupJoin(solicitudesPorMesData,
                       mesCompleto => new { mesCompleto.Anio, mesCompleto.Mes },
                       datoSolicitud => new { datoSolicitud.Anio, datoSolicitud.Mes },
                       (mesCompleto, grupoSolicitudes) => new SolicitudMensualViewModel
                       {
                         MesNombre = mesCompleto.MesNombre,
                         Anio = mesCompleto.Anio,
                         Mes = mesCompleto.Mes,
                         Cantidad = grupoSolicitudes.FirstOrDefault()?.Cantidad ?? 0
                       })
            .OrderBy(x => x.Anio).ThenBy(x => x.Mes)
            .ToList();

        viewModel.UltimasSolicitudesRecibidas = await _context.Solicitudes
            .Include(s => s.NivelesIdioma)
            .OrderByDescending(s => s.FechaEnvioSolicitud)
            .Take(NumeroItemsRecientes)
            .ToListAsync();

        viewModel.TiposDeSolicitudConteo = await _context.Solicitudes
            .GroupBy(s => s.TipoSolicitud)
            .Select(g => new TipoSolicitudConteoViewModel { Tipo = g.Key, Cantidad = g.Count() })
            .OrderBy(t => t.Tipo)
            .ToListAsync();

        viewModel.ProgramasPorEstado = await _context.ProgramasProyectosONG
            .Where(p => !string.IsNullOrEmpty(p.EstadoProgramaProyecto))
            .GroupBy(p => p.EstadoProgramaProyecto!)
            .Select(g => new ProgramaPorEstadoViewModel { Estado = g.Key, Cantidad = g.Count() })
            .OrderBy(p => p.Estado)
            .ToListAsync();

        viewModel.ProximosProgramas = await _context.ProgramasProyectosONG
            .Where(p => p.FechaInicioEstimada >= DateTime.Today && (p.EstadoProgramaProyecto == "Planificación" || p.EstadoProgramaProyecto == "En Curso"))
            .OrderBy(p => p.FechaInicioEstimada)
            .Take(3)
            .ToListAsync();

        viewModel.BeneficiariosRecientes = await _context.Beneficiarios
            .Include(b => b.Comunidad)
            .OrderByDescending(b => b.FechaRegistroBeneficiario)
            .Take(NumeroItemsRecientes)
            .ToListAsync();
      }
      else // --- LÓGICA PARA USUARIO NORMAL ---
      {
        // KPIs específicos del usuario
        viewModel.MisSolicitudesVolPasCount = await _context.Solicitudes
            .CountAsync(s => s.UsuarioCreadorId == currentUserId);
        viewModel.MisConsultasGeneralesCount = await _context.SolicitudesInformacionGeneral
            .CountAsync(s => s.UsuarioCreadorId == currentUserId);

        var participacionesUsuario = _context.ParticipacionesActivas
            .Include(p => p.ProgramaProyecto)
            .Include(p => p.Solicitud)
            .Where(p => p.Solicitud != null && p.Solicitud.UsuarioCreadorId == currentUserId);

        viewModel.MisParticipacionesActivasCount = await participacionesUsuario
            .CountAsync(p => p.ProgramaProyecto != null && (p.ProgramaProyecto.EstadoProgramaProyecto == "En Curso" || p.ProgramaProyecto.EstadoProgramaProyecto == "Planificación"));

        // Evaluaciones pendientes: participaciones del usuario que no tienen una evaluación asociada
        viewModel.MisEvaluacionesPendientesDetalles = await participacionesUsuario
            .Where(p => !p.EvaluacionesPrograma.Any() && (p.FechaFinParticipacion.HasValue && p.FechaFinParticipacion.Value <= DateTime.UtcNow || p.ProgramaProyecto!.EstadoProgramaProyecto == "En Curso")) // Finalizadas o aún en curso
            .OrderByDescending(p => p.FechaInicioParticipacion)
            .Select(p => new ParticipacionParaEvaluarViewModel
            {
              ParticipacionId = p.ParticipacionID,
              NombrePrograma = p.ProgramaProyecto!.NombreProgramaProyecto,
              NombreSolicitante = p.Solicitud!.Nombres + " " + p.Solicitud.Apellidos,
              FechaFinParticipacion = p.FechaFinParticipacion,
              EstadoParticipacion = p.ProgramaProyecto.EstadoProgramaProyecto, // O un estado de la participación si lo tienes
              SolicitudId = p.SolicitudID
            })
            .Take(NumeroItemsRecientes)
            .ToListAsync();
        viewModel.MisEvaluacionesPendientesCount = viewModel.MisEvaluacionesPendientesDetalles.Count;


        // Listas específicas del usuario
        viewModel.MisUltimasSolicitudesVolPas = await _context.Solicitudes
            .Where(s => s.UsuarioCreadorId == currentUserId)
            .Include(s => s.NivelesIdioma)
            .OrderByDescending(s => s.FechaEnvioSolicitud)
            .Take(NumeroItemsRecientes)
            .ToListAsync();

        viewModel.MisProgramasAsignados = await participacionesUsuario
            .Where(p => p.ProgramaProyecto != null && (p.ProgramaProyecto.EstadoProgramaProyecto == "En Curso" || p.ProgramaProyecto.EstadoProgramaProyecto == "Planificación"))
            .OrderByDescending(p => p.FechaInicioParticipacion)
            .Take(NumeroItemsRecientes)
            .ToListAsync();
      }

      ViewData["title"] = "Dashboard Principal";
      ViewData["pageTitle"] = "Dashboard";

      return View(viewModel);
    }
  }
}
