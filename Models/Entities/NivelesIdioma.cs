using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("NivelesIdioma")]
  public class NivelesIdioma
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NivelIdiomaID { get; set; }

    [Required(ErrorMessage = "El nombre del nivel de idioma es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Nombre del Nivel")]
    public string NombreNivel { get; set; } = null!;

    // Propiedad de navegación: Un NivelIdioma puede estar asociado a muchas Solicitudes
    public virtual ICollection<Solicitudes> Solicitudes { get; set; } = new List<Solicitudes>();
  }
}
