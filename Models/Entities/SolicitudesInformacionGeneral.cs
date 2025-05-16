// VN_Center/Models/Entities/SolicitudesInformacionGeneral.cs
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
    public DateTime FechaRecepcion { get; set; }

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
    public string? ProgramaDeInteres { get; set; }

    [StringLength(255)]
    [Display(Name = "Otro Programa (especificar)")]
    public string? ProgramaDeInteresOtro { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Preguntas Específicas")]
    [DataType(DataType.MultilineText)]
    public string? PreguntasEspecificas { get; set; }

    [Required(ErrorMessage = "El estado de la solicitud es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Estado de la Solicitud")]
    public string EstadoSolicitudInfo { get; set; } // Valor por defecto en constructor

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas de Seguimiento")]
    [DataType(DataType.MultilineText)]
    public string? NotasSeguimiento { get; set; }

    [Display(Name = "Usuario Asignado")]
    public int? UsuarioAsignadoID { get; set; }

    [Display(Name = "Fuente de Conocimiento")]
    public int? FuenteConocimientoID { get; set; }

    // --- NUEVA PROPIEDAD PARA AUDITORÍA Y FILTRADO ---
    [StringLength(450)]
    [Display(Name = "Usuario Creador ID")]
    public string? UsuarioCreadorId { get; set; }
    // --- FIN DE NUEVA PROPIEDAD ---

    // --- Propiedades de Navegación ---
    [ForeignKey("UsuarioAsignadoID")]
    public virtual UsuariosSistema? UsuarioAsignado { get; set; }

    [ForeignKey("FuenteConocimientoID")]
    public virtual FuentesConocimiento? FuenteConocimiento { get; set; }

    public SolicitudesInformacionGeneral()
    {
      FechaRecepcion = DateTime.UtcNow;
      EstadoSolicitudInfo = "Nueva"; // Valor inicial por defecto
    }
  }
}
