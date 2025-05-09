using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("SolicitudesInformacionGeneral")]
  public class SolicitudesInformacionGeneral
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SolicitudInfoID { get; set; }

    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(200)]
    public string NombreContacto { get; set; } = null!;

    [Required]
    [StringLength(254)]
    public string EmailContacto { get; set; } = null!;

    [StringLength(30)]
    public string? TelefonoContacto { get; set; }

    public bool? PermiteContactoWhatsApp { get; set; }

    [StringLength(255)]
    public string? ProgramaDeInteres { get; set; }

    [StringLength(255)]
    public string? ProgramaDeInteresOtro { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? PreguntasEspecificas { get; set; }

    [Required]
    [StringLength(50)]
    public string EstadoSolicitudInfo { get; set; } = "Nueva";

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? NotasSeguimiento { get; set; }

    public int? UsuarioAsignadoID { get; set; } // FK

    // --- Propiedades de Navegación ---
    [ForeignKey("UsuarioAsignadoID")]
    public virtual UsuariosSistema? UsuarioAsignado { get; set; }
  }
}
