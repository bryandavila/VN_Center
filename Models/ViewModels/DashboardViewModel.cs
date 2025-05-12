using System.Collections.Generic; // Necesario para List<>
using System.ComponentModel.DataAnnotations;
using VN_Center.Models.Entities; // Para List<Solicitudes>

namespace VN_Center.Models.ViewModels
{
  public class DashboardViewModel
  {
    [Display(Name = "Solicitudes Totales")]
    public int TotalSolicitudes { get; set; }

    [Display(Name = "Beneficiarios Totales")]
    public int TotalBeneficiarios { get; set; }

    [Display(Name = "Programas/Proyectos Activos")]
    public int TotalProgramasActivos { get; set; }

    [Display(Name = "Comunidades Atendidas")]
    public int TotalComunidades { get; set; }

    // Nuevas propiedades para el dashboard mejorado
    [Display(Name = "Solicitudes por Mes")]
    public List<SolicitudMensualViewModel> SolicitudesPorMes { get; set; } = new List<SolicitudMensualViewModel>();

    [Display(Name = "Últimas Solicitudes Recibidas")]
    public List<Solicitudes> UltimasSolicitudesRecibidas { get; set; } = new List<Solicitudes>();

    // Aquí podrías añadir más propiedades en el futuro:
    // public List<BeneficiariosPorComunidadViewModel> BeneficiariosPorComunidad { get; set; }
    // public List<ProgramasPorEstadoViewModel> ProgramasPorEstado { get; set; }
    // public List<ProgramasProyectosONG> ProximosProgramasAIniciar { get; set; }
  }
}
