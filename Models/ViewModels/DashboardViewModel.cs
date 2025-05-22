using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VN_Center.Models.Entities;

namespace VN_Center.Models.ViewModels
{
  public class DashboardViewModel
  {
    // --- Propiedades para el Dashboard de Administrador (existentes) ---
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

    // --- NUEVAS PROPIEDADES PARA EL DASHBOARD DE USUARIO NORMAL ---
    [Display(Name = "Mis Solicitudes (Vol/Pas)")]
    public int MisSolicitudesVolPasCount { get; set; }

    [Display(Name = "Mis Consultas Generales")]
    public int MisConsultasGeneralesCount { get; set; }

    [Display(Name = "Mis Participaciones Activas")]
    public int MisParticipacionesActivasCount { get; set; }

    [Display(Name = "Evaluaciones Pendientes")]
    public int MisEvaluacionesPendientesCount { get; set; }

    [Display(Name = "Mis Últimas Solicitudes (Vol/Pas)")]
    public List<Solicitudes> MisUltimasSolicitudesVolPas { get; set; } = new List<Solicitudes>();

    [Display(Name = "Mis Programas Asignados")]
    public List<ParticipacionesActivas> MisProgramasAsignados { get; set; } = new List<ParticipacionesActivas>();

    [Display(Name = "Mis Evaluaciones de Programa Pendientes")]
    public List<ParticipacionParaEvaluarViewModel> MisEvaluacionesPendientesDetalles { get; set; } = new List<ParticipacionParaEvaluarViewModel>();
  }

  // ViewModel auxiliar para mostrar participaciones que necesitan evaluación
  public class ParticipacionParaEvaluarViewModel
  {
    public int ParticipacionId { get; set; }
    public string? NombrePrograma { get; set; }
    public string? NombreSolicitante { get; set; }
    public DateTime? FechaFinParticipacion { get; set; }
    public string? EstadoParticipacion { get; set; }
    public int SolicitudId { get; set; } // Para el enlace
  }
}
