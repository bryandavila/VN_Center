using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Necesario para [Display]
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
    [Display(Name = "Nombre de la Comunidad")] // Nombre a mostrar para NombreComunidad
    public string NombreComunidad { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ubicación Detallada")] // Nombre a mostrar para UbicacionDetallada
    public string? UbicacionDetallada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas Adicionales")] // Nombre a mostrar para NotasComunidad
    public string? NotasComunidad { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Beneficiarios> Beneficiarios { get; set; } = new List<Beneficiarios>();
    public virtual ICollection<GruposComunitarios> GruposComunitarios { get; set; } = new List<GruposComunitarios>();
    public virtual ICollection<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = new List<ProgramaProyectoComunidades>();
  }
}
