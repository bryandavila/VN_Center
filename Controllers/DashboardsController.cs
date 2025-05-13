using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.ViewModels;
using System.Globalization;
using VN_Center.Models.Entities; // Necesario para acceder a las entidades directamente
using Microsoft.AspNetCore.Authorization;

namespace VN_Center.Controllers
{
  [Authorize]
  public class DashboardsController : Controller
  {
    private readonly VNCenterDbContext _context;
    private const int NumeroItemsRecientes = 5; // Constante para listas de actividad

    public DashboardsController(VNCenterDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      var viewModel = new DashboardViewModel
      {
        TotalSolicitudes = await _context.Solicitudes.CountAsync(),
        TotalBeneficiarios = await _context.Beneficiarios.CountAsync(),
        TotalProgramasActivos = await _context.ProgramasProyectosONG
                                          .CountAsync(p => p.EstadoProgramaProyecto == "En Curso" || p.EstadoProgramaProyecto == "Planificación"),
        TotalComunidades = await _context.Comunidades.CountAsync()
      };

      // --- Datos para Gráfico de Solicitudes por Mes (últimos 12 meses) ---
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
          .Select(d => new { Anio = d.Year, Mes = d.Month, MesNombre = d.ToString("MMM yy", culturaES) }) // Formato Mes Año
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

      // --- Datos para Lista de Últimas Solicitudes Recibidas (Top 5) ---
      viewModel.UltimasSolicitudesRecibidas = await _context.Solicitudes
          .Include(s => s.NivelesIdioma)
          .OrderByDescending(s => s.FechaEnvioSolicitud)
          .Take(NumeroItemsRecientes)
          .ToListAsync();

      // --- Datos para Gráfico Circular de Tipos de Solicitud ---
      viewModel.TiposDeSolicitudConteo = await _context.Solicitudes
          .GroupBy(s => s.TipoSolicitud)
          .Select(g => new TipoSolicitudConteoViewModel { Tipo = g.Key, Cantidad = g.Count() })
          .OrderBy(t => t.Tipo)
          .ToListAsync();

      // --- Datos para Gráfico de Programas por Estado ---
      viewModel.ProgramasPorEstado = await _context.ProgramasProyectosONG
          .Where(p => !string.IsNullOrEmpty(p.EstadoProgramaProyecto)) // Evitar estados nulos o vacíos
          .GroupBy(p => p.EstadoProgramaProyecto)
          .Select(g => new ProgramaPorEstadoViewModel { Estado = g.Key!, Cantidad = g.Count() }) // Usar g.Key! si EstadoProgramaProyecto no es nullable
          .OrderBy(p => p.Estado)
          .ToListAsync();

      // --- Datos para Lista de Próximos Inicios de Programas (Top 3) ---
      viewModel.ProximosProgramas = await _context.ProgramasProyectosONG
          .Where(p => p.FechaInicioEstimada >= DateTime.Today && (p.EstadoProgramaProyecto == "Planificación" || p.EstadoProgramaProyecto == "En Curso"))
          .OrderBy(p => p.FechaInicioEstimada)
          .Take(3)
          .ToListAsync();

      // --- Datos para Lista de Beneficiarios Recientemente Registrados (Top 5) ---
      viewModel.BeneficiariosRecientes = await _context.Beneficiarios
          .Include(b => b.Comunidad) // Incluir comunidad si quieres mostrarla
          .OrderByDescending(b => b.FechaRegistroBeneficiario)
          .Take(NumeroItemsRecientes)
          .ToListAsync();


      ViewData["title"] = "Dashboard Principal";
      ViewData["pageTitle"] = "Dashboard";

      return View(viewModel);
    }
  }
}
