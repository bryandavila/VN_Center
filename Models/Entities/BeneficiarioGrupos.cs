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
    [Display(Name = "Beneficiario")]
    public int BeneficiarioID { get; set; }

    [Key]
    [Column(Order = 1)]
    [Display(Name = "Grupo Comunitario")]
    public int GrupoID { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Unión al Grupo")]
    [DataType(DataType.Date)]
    public DateTime? FechaUnionGrupo { get; set; }

    [StringLength(100)]
    [Display(Name = "Rol en el Grupo")]
    public string? RolEnGrupo { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("BeneficiarioID")]
    public virtual Beneficiarios Beneficiario { get; set; } = null!;

    [ForeignKey("GrupoID")]
    public virtual GruposComunitarios GrupoComunitario { get; set; } = null!;
  }
}
