using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("BeneficiariosProgramasProyectos")]
  public class BeneficiariosProgramasProyectos
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    [Display(Name = "Beneficiario")]
    public int BeneficiarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    [Display(Name = "Programa/Proyecto")]
    public int ProgramaProyectoID { get; set; }

    [Required(ErrorMessage = "La fecha de inscripción es obligatoria.")]
    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Inscripción")]
    [DataType(DataType.Date)]
    public DateTime FechaInscripcionBeneficiario { get; set; } = DateTime.Today;

    [StringLength(50)]
    [Display(Name = "Estado de Participación")]
    public string? EstadoParticipacionBeneficiario { get; set; } // Ej: 'Activo', 'Completado', 'Retirado'

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas Adicionales")]
    [DataType(DataType.MultilineText)]
    public string? NotasAdicionales { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("BeneficiarioID")]
    public virtual Beneficiarios Beneficiario { get; set; } = null!;

    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;
  }
}
