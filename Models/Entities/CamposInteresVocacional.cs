using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("CamposInteresVocacional")]
  public class CamposInteresVocacional
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CampoInteresID { get; set; }

    [Required(ErrorMessage = "El nombre del campo de interés es obligatorio.")]
    [StringLength(200)]
    [Display(Name = "Nombre del Campo de Interés")]
    public string NombreCampo { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción del Campo")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionCampo { get; set; }

    // Propiedad de navegación para la tabla de cruce SolicitudCamposInteres
    public virtual ICollection<SolicitudCamposInteres> SolicitudCamposInteres { get; set; } = new List<SolicitudCamposInteres>();
  }
}
