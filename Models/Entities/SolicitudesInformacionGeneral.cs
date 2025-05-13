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
    public string NombreContacto { get; set; } = null!; // Usado por las vistas

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
    [StringLength(254)]
    [Display(Name = "Correo Electrónico")]
    public string EmailContacto { get; set; } = null!; // Usado por las vistas

    [StringLength(30)]
    [Display(Name = "Teléfono de Contacto")]
    public string? TelefonoContacto { get; set; } // Usado por las vistas

    [Display(Name = "Permite Contacto por WhatsApp")]
    public bool? PermiteContactoWhatsApp { get; set; } // Usado por las vistas

    [StringLength(255)]
    [Display(Name = "Programa de Interés")]
    public string? ProgramaDeInteres { get; set; } // Usado por las vistas

    [StringLength(255)]
    [Display(Name = "Otro Programa (especificar)")]
    public string? ProgramaDeInteresOtro { get; set; } // Usado por las vistas

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Preguntas Específicas")]
    [DataType(DataType.MultilineText)]
    public string? PreguntasEspecificas { get; set; } // Usado por las vistas (en lugar de 'Mensaje')

    [Required(ErrorMessage = "El estado de la solicitud es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Estado de la Solicitud")]
    public string EstadoSolicitudInfo { get; set; } = "Nueva"; // Usado por las vistas (en lugar de 'EstadoSolicitud')

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Notas de Seguimiento")]
    [DataType(DataType.MultilineText)]
    public string? NotasSeguimiento { get; set; } // Usado por las vistas

    // Relación con Usuarios (el usuario asignado a la solicitud)
    [Display(Name = "Usuario Asignado")]
    public int? UsuarioAsignadoID { get; set; }
    [ForeignKey("UsuarioAsignadoID")]
    public virtual UsuariosSistema? UsuarioAsignado { get; set; }

    // Relación con FuentesConocimiento (necesaria para el controlador)
    [Display(Name = "Fuente de Conocimiento")]
    public int? FuenteConocimientoID { get; set; }
    public virtual FuentesConocimiento? FuenteConocimiento { get; set; }

    // Propiedades que estaban en la versión anterior de la entidad y que el controlador `solicitudes_info_controller_final` usa en su Bind.
    // Si no son usadas por las vistas actuales, pueden ser redundantes o necesitar un mapeo si la lógica de negocio las requiere.
    // Por ahora, las comentaré para priorizar lo que las vistas usan. Si el controlador las necesita explícitamente más allá del Bind,
    // tendríamos que decidir si las vistas también deben mostrarlas o si el controlador debe adaptarse.

    /*
    // Estas propiedades estaban en la entidad que el controlador 'solicitudes_info_controller_final' esperaba:
    // Si tus vistas actuales NO las usan, y la lógica de negocio tampoco, considera si son necesarias.
    // Si las vistas SÍ las necesitan con estos nombres, descoméntalas y ajusta las vistas.

    [Required(ErrorMessage = "Los nombres del solicitante son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Nombres del Solicitante")] // Alternativa a NombreContacto
    public string NombresSolicitante { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos del solicitante son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Apellidos del Solicitante")] // Alternativa a NombreContacto
    public string ApellidosSolicitante { get; set; } = null!;

    [StringLength(150)]
    [Display(Name = "Organización (Si aplica)")]
    public string? Organizacion { get; set; }

    // Email es EmailContacto arriba
    // Telefono es TelefonoContacto arriba

    [Required(ErrorMessage = "El mensaje o consulta es obligatorio.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Mensaje/Consulta")] // Alternativa a PreguntasEspecificas
    public string Mensaje { get; set; } = null!;

    // FechaSolicitud es FechaRecepcion arriba
    // EstadoSolicitud es EstadoSolicitudInfo arriba

    [DataType(DataType.DateTime)]
    [Display(Name = "Fecha de Respuesta")]
    public DateTime? FechaRespuesta { get; set; }

    // NotasInternas es NotasSeguimiento arriba
    */


    public SolicitudesInformacionGeneral()
    {
      FechaRecepcion = DateTime.UtcNow;
      EstadoSolicitudInfo = "Nueva";
    }
  }
}
