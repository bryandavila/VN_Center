using System.Collections.Generic;
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

    [Required(ErrorMessage = "El nombre de la fuente es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre de la Fuente")]
    public string NombreFuente { get; set; } = null!;

    // Propiedad de navegación
    public virtual ICollection<Solicitudes> Solicitudes { get; set; } = new List<Solicitudes>();
  }
}
