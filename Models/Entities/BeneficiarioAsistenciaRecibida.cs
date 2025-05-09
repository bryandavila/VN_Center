using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("BeneficiarioAsistenciaRecibida")]
  public class BeneficiarioAsistenciaRecibida
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    public int BeneficiarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    public int TipoAsistenciaID { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? NotasAdicionales { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("BeneficiarioID")]
    public virtual Beneficiarios Beneficiario { get; set; } = null!;

    [ForeignKey("TipoAsistenciaID")]
    public virtual TiposAsistencia TipoAsistencia { get; set; } = null!;
  }
}
