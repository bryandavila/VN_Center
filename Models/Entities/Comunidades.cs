using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("Comunidades")]
  public class Comunidades
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ComunidadID { get; set; }

    [Required]
    [StringLength(150)]
    public string NombreComunidad { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? UbicacionDetallada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? NotasComunidad { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Beneficiarios> Beneficiarios { get; set; } = new List<Beneficiarios>();
    public virtual ICollection<GruposComunitarios> GruposComunitarios { get; set; } = new List<GruposComunitarios>();
    public virtual ICollection<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = new List<ProgramaProyectoComunidades>();
  }
}
