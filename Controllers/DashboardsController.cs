using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.ViewModels;
using System.Globalization; // Para CultureInfo

namespace VN_Center.Controllers
{
  public class DashboardsController : Controller
  {
    private readonly VNCenterDbContext _context;

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
          .Where(s => s.FechaEnvioSolicitud >= haceUnAnio) // Filtrar por el último año
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

      // Convertir Mes (int) a NombreMes para el ViewModel y asegurar todos los meses
      var culturaES = new CultureInfo("es-ES"); // Cultura para nombres de mes en español
      var todosLosMeses = Enumerable.Range(0, 12)
          .Select(i => DateTime.UtcNow.AddMonths(-i))
          .Select(d => new { Anio = d.Year, Mes = d.Month, MesNombre = d.ToString("MMM yyyy", culturaES) })
          .OrderBy(x => x.Anio).ThenBy(x => x.Mes)
          .ToList();

      viewModel.SolicitudesPorMes = todosLosMeses
          .GroupJoin(solicitudesPorMesData, // Unir con los datos reales
                     mesCompleto => new { mesCompleto.Anio, mesCompleto.Mes },
                     datoSolicitud => new { datoSolicitud.Anio, datoSolicitud.Mes },
                     (mesCompleto, grupoSolicitudes) => new SolicitudMensualViewModel
                     {
                       MesNombre = mesCompleto.MesNombre,
                       Anio = mesCompleto.Anio,
                       Mes = mesCompleto.Mes,
                       Cantidad = grupoSolicitudes.FirstOrDefault()?.Cantidad ?? 0 // Si no hay solicitudes, cantidad es 0
                     })
          .OrderBy(x => x.Anio).ThenBy(x => x.Mes) // Asegurar el orden final
          .ToList();


      // --- Datos para Lista de Últimas Solicitudes Recibidas (Top 5) ---
      viewModel.UltimasSolicitudesRecibidas = await _context.Solicitudes
          .Include(s => s.NivelesIdioma) // Opcional, para mostrar más info si quieres
          .OrderByDescending(s => s.FechaEnvioSolicitud)
          .Take(5)
          .ToListAsync();

      ViewData["title"] = "Dashboard Principal";
      ViewData["pageTitle"] = "Dashboard";

      return View(viewModel);
    }
  }
}
