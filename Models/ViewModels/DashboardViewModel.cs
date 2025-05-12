using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VN_Center.Models.Entities; // Para List<Solicitudes>, List<ProgramasProyectosONG>, List<Beneficiarios>

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

    [Display(Name = "Solicitudes por Mes")]
    public List<SolicitudMensualViewModel> SolicitudesPorMes { get; set; } = new List<SolicitudMensualViewModel>();

    [Display(Name = "Últimas Solicitudes Recibidas")]
    public List<Solicitudes> UltimasSolicitudesRecibidas { get; set; } = new List<Solicitudes>();

    [Display(Name = "Distribución por Tipo de Solicitud")]
    public List<TipoSolicitudConteoViewModel> TiposDeSolicitudConteo { get; set; } = new List<TipoSolicitudConteoViewModel>();

    [Display(Name = "Estado de Programas/Proyectos")]
    public List<ProgramaPorEstadoViewModel> ProgramasPorEstado { get; set; } = new List<ProgramaPorEstadoViewModel>();

    [Display(Name = "Próximos Inicios de Programas")]
    public List<ProgramasProyectosONG> ProximosProgramas { get; set; } = new List<ProgramasProyectosONG>();

    [Display(Name = "Beneficiarios Recientemente Registrados")]
    public List<Beneficiarios> BeneficiariosRecientes { get; set; } = new List<Beneficiarios>();
  }
}
