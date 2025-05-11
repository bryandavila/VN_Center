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

    [Display(Name = "Fecha de Recepción")]
    [DataType(DataType.DateTime)]
    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "El nombre de contacto es obligatorio.")]
    [StringLength(200)]
    [Display(Name = "Nombre del Contacto")]
    public string NombreContacto { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
    [StringLength(254)]
    [Display(Name = "Correo Electrónico")]
    public string EmailContacto { get; set; } = null!;

    [StringLength(30)]
    [Display(Name = "Teléfono de Contacto")]
    public string? TelefonoContacto { get; set; }

    [Display(Name = "Permite Contacto por WhatsApp")]
    public bool? PermiteContactoWhatsApp { get; set; }

    [StringLength(255)]
    [Display(Name = "Programa de Interés")]
    public string? ProgramaDeInteres { get; set; } // Podría ser un dropdown si los programas son fijos y pocos

    [StringLength(255)]
    [Display(Name = "Otro Programa (especificar)")]
    public string? ProgramaDeInteresOtro { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Preguntas Específicas")]
    [DataType(DataType.MultilineText)]
    public string? PreguntasEspecificas { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Estado de la Solicitud")]
    public string EstadoSolicitudInfo { get; set; } = "Nueva"; // Ej: Nueva, En Proceso, Respondida, Cerrada

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas de Seguimiento")]
    [DataType(DataType.MultilineText)]
    public string? NotasSeguimiento { get; set; }

    [Display(Name = "Usuario Asignado (ONG)")]
    public int? UsuarioAsignadoID { get; set; } // FK

    // --- Propiedades de Navegación ---
    [ForeignKey("UsuarioAsignadoID")]
    [Display(Name = "Usuario Asignado")]
    public virtual UsuariosSistema? UsuarioAsignado { get; set; }
  }
}
