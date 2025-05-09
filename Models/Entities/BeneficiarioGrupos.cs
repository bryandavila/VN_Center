using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("BeneficiarioGrupos")]
  public class BeneficiarioGrupos
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    public int BeneficiarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    public int GrupoID { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaUnionGrupo { get; set; }

    [StringLength(100)]
    public string? RolEnGrupo { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("BeneficiarioID")]
    public virtual Beneficiarios Beneficiario { get; set; } = null!;

    [ForeignKey("GrupoID")]
    public virtual GruposComunitarios GrupoComunitario { get; set; } = null!;
  }
}
