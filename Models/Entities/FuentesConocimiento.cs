using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("FuentesConocimiento")]
  public class FuentesConocimiento
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FuenteConocimientoID { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreFuente { get; set; } = null!;

    // Propiedad de navegación
    public virtual ICollection<Solicitudes> Solicitudes { get; set; } = new List<Solicitudes>();
  }
}
