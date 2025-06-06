using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("TiposAsistencia")]
  public class TiposAsistencia
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TipoAsistenciaID { get; set; }

    [Required(ErrorMessage = "El nombre del tipo de asistencia es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre del Tipo de Asistencia")]
    public string NombreAsistencia { get; set; } = null!;

    // Propiedad de navegación para la tabla de cruce BeneficiarioAsistenciaRecibida
    public virtual ICollection<BeneficiarioAsistenciaRecibida> BeneficiarioAsistenciaRecibida { get; set; } = new List<BeneficiarioAsistenciaRecibida>();
  }
}
