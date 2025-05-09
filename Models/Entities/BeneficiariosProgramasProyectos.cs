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
    public int BeneficiarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    public int ProgramaProyectoID { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime FechaInscripcionBeneficiario { get; set; } = DateTime.Today;

    [StringLength(50)]
    public string? EstadoParticipacionBeneficiario { get; set; } // "Activo", "Completado", "Retirado"

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? NotasAdicionales { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("BeneficiarioID")]
    public virtual Beneficiarios Beneficiario { get; set; } = null!;

    [ForeignKey("ProgramaProyectoID")]
    public virtual ProgramasProyectosONG ProgramaProyecto { get; set; } = null!;
  }
}
