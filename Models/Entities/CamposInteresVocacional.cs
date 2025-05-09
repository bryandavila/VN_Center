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

    [Required]
    [StringLength(200)]
    public string NombreCampo { get; set; } = null!;

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? DescripcionCampo { get; set; }

    // Propiedad de navegación para la tabla de cruce SolicitudCamposInteres
    public virtual ICollection<SolicitudCamposInteres> SolicitudCamposInteres { get; set; } = new List<SolicitudCamposInteres>();
  }
}
