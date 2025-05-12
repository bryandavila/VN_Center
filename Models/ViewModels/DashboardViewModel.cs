using System.ComponentModel.DataAnnotations; // Para [Display]

namespace VN_Center.Models.ViewModels
{
  public class DashboardViewModel
  {
    [Display(Name = "Solicitudes Totales")]
    public int TotalSolicitudes { get; set; }

    [Display(Name = "Beneficiarios Totales")]
    public int TotalBeneficiarios { get; set; }

    [Display(Name = "Programas/Proyectos Activos")]
    public int TotalProgramasActivos { get; set; } // Podríamos definir "Activo" según el campo Estado

    [Display(Name = "Comunidades Atendidas")]
    public int TotalComunidades { get; set; }

    // Aquí podrías añadir más propiedades para otras estadísticas en el futuro
    // Ejemplo:
    // public int VoluntariosActivos { get; set; }
    // public int PasantiasEnCurso { get; set; }
  }
}
