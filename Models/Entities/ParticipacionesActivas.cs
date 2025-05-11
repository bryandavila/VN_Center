using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("ParticipacionesActivas")]
  public class ParticipacionesActivas
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ParticipacionID { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una solicitud (voluntario/pasante).")]
    [Display(Name = "Solicitante (Voluntario/Pasante)")]
    public int SolicitudID { get; set; } // FK

    [Required(ErrorMessage = "Debe seleccionar un programa o proyecto.")]
    [Display(Name = "Programa/Proyecto Asignado")]
    public int ProgramaProyectoID { get; set; } // FK

    [Required(ErrorMessage = "La fecha de asignación es obligatoria.")]
    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Asignación")]
    [DataType(DataType.Date)]
    public DateTime FechaAsignacion { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La fecha de inicio de participación es obligatoria.")]
    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha Inicio Participación")]
    [DataType(DataType.Date)]
    public DateTime FechaInicioParticipacion { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha Fin Participación (Opcional)")]
    [DataType(DataType.Date)]
    public DateTime? FechaFinParticipacion { get; set; }

    [StringLength(150)]
    [Display(Name = "Rol Desempeñado")]
    public string? RolDesempenado { get; set; }

    [Display(Name = "Horas TCU Completadas")]
    [Range(0, 1000, ErrorMessage = "Las horas deben ser un valor positivo.")] // Ajusta el rango según sea necesario
    public int? HorasTCUCompletadas { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas del Supervisor")]
    [DataType(DataType.MultilineText)]
    public string? NotasSupervisor { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("SolicitudID")]
    public virtual Solicitudes Solicitud { get; set; } = null!;

    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;

    public virtual ICollection<EvaluacionesPrograma> EvaluacionesPrograma { get; set; } = new List<EvaluacionesPrograma>();
  }
}
