using System;
using System.Linq;
using System.Threading.Tasks; // Necesario para Task y ToListAsync
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync y CountAsync
using VN_Center.Data; // Namespace de tu DbContext
using VN_Center.Models.ViewModels; // Namespace de tu ViewModel

namespace VN_Center.Controllers
{
  public class DashboardsController : Controller
  {
    private readonly VNCenterDbContext _context;

    public DashboardsController(VNCenterDbContext context)
    {
      _context = context; // Inyectar el DbContext
    }

    public async Task<IActionResult> Index()
    {
      var viewModel = new DashboardViewModel
      {
        TotalSolicitudes = await _context.Solicitudes.CountAsync(),
        TotalBeneficiarios = await _context.Beneficiarios.CountAsync(),
        // Para programas activos, necesitas definir qué significa "Activo"
        // Asumiremos que es un estado específico, por ejemplo "En Curso"
        TotalProgramasActivos = await _context.ProgramasProyectosONG
                                          .CountAsync(p => p.EstadoProgramaProyecto == "En Curso" || p.EstadoProgramaProyecto == "Planificación"),
        TotalComunidades = await _context.Comunidades.CountAsync()
      };

      // Puedes pasar más datos a través de ViewData si es necesario para la plantilla original
      ViewData["title"] = "Dashboard Principal";
      ViewData["pageTitle"] = "Dashboard"; // O algo más específico

      return View(viewModel);
    }

    // Si tenías otras acciones en este controlador de la plantilla, puedes mantenerlas o eliminarlas.
    // Por ejemplo, si había una acción para "CRM", "eCommerce", etc., que ya no aplican.
    // public IActionResult Analytics() => View(); // Esta podría ser la que ya tienes como Index
    // public IActionResult Crm() => View();
    // public IActionResult Ecommerce() => View();
  }
}
