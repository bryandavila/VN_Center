using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("SolicitudCamposInteres")]
  public class SolicitudCamposInteres
  {
    // Clave primaria compuesta
    [Key]
    [Column(Order = 0)]
    public int SolicitudID { get; set; }

    [Key]
    [Column(Order = 1)]
    public int CampoInteresID { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("SolicitudID")]
    public virtual Solicitudes Solicitud { get; set; } = null!;

    [ForeignKey("CampoInteresID")]
    public virtual CamposInteresVocacional CampoInteresVocacional { get; set; } = null!;
  }
}
