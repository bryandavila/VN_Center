using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("Solicitudes")]
  public class Solicitudes
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SolicitudID { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [Display(Name = "Permite Contacto por WhatsApp")]
    public bool? PermiteContactoWhatsApp { get; set; } // Se manejará con radios/checkbox en la vista

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Nacimiento")]
    [DataType(DataType.Date)]
    public DateTime? FechaNacimiento { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Tipo de Solicitud")]
    public string TipoSolicitud { get; set; } = null!; // Se manejará con dropdown en la vista

    [Display(Name = "Fecha de Envío")]
    [DataType(DataType.DateTime)]
    public DateTime FechaEnvioSolicitud { get; set; } = DateTime.UtcNow;

    [Display(Name = "Pasaporte Válido (+6 meses)")]
    public bool? PasaporteValidoSeisMeses { get; set; } // Se manejará con radios/checkbox en la vista

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha Expiración Pasaporte")]
    [DataType(DataType.Date)]
    public DateTime? FechaExpiracionPasaporte { get; set; }

    [StringLength(255)]
    [Display(Name = "Profesión u Ocupación")]
    public string? ProfesionOcupacion { get; set; }

    [Display(Name = "Nivel de Español")]
    public int? NivelIdiomaEspañolID { get; set; }

    [StringLength(255)]
    [Display(Name = "Otros Idiomas Hablados")]
    public string? OtrosIdiomasHablados { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Experiencia Previa (Voluntariado/Pasantía/Servicio)", Prompt = "Detalla cualquier experiencia previa relevante.")]
    [DataType(DataType.MultilineText)]
    public string? ExperienciaPreviaSVL { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Experiencia Laboral Relevante", Prompt = "Describe tu experiencia laboral que consideres importante.")]
    [DataType(DataType.MultilineText)]
    public string? ExperienciaLaboralRelevante { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Habilidades Relevantes", Prompt = "Ej: viajes, trabajo en equipo, comunicación, investigación, habilidades técnicas, etc.")]
    [DataType(DataType.MultilineText)]
    public string? HabilidadesRelevantes { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Inicio Preferida", Prompt = "Fecha aproximada en la que te gustaría comenzar.")]
    [DataType(DataType.Date)]
    public DateTime? FechaInicioPreferida { get; set; }

    [StringLength(100)]
    [Display(Name = "Duración del Programa", Prompt = "Ej: 2 semanas, 1 mes, 3 meses")]
    public string? DuracionEstancia { get; set; }

    [StringLength(100)]
    [Display(Name = "Otra Duración (especificar)")]
    public string? DuracionEstanciaOtro { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Motivación e Interés en Costa Rica", Prompt = "¿Qué te atrae de Costa Rica y por qué quieres ser voluntario/pasante aquí?")]
    [DataType(DataType.MultilineText)]
    public string? MotivacionInteresCR { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Descripción: Salir de tu Zona de Confort", Prompt = "Describe lo que significa para ti salir de tu zona de confort.")]
    [DataType(DataType.MultilineText)]
    public string? DescripcionSalidaZonaConfort { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Información Adicional Personal", Prompt = "Cualquier otra información relevante sobre tus intereses o cualificaciones.")]
    [DataType(DataType.MultilineText)]
    public string? InformacionAdicionalPersonal { get; set; }

    [Display(Name = "¿Cómo nos conoció?")]
    public int? FuenteConocimientoID { get; set; }

    [StringLength(255)]
    [Display(Name = "Otra Fuente (especificar)")]
    public string? FuenteConocimientoOtro { get; set; }

    [StringLength(512)]
    [Display(Name = "Ruta CV (Archivo)")]
    public string? PathCV { get; set; }

    [StringLength(512)]
    [Display(Name = "Ruta Carta Recomendación (Archivo)")]
    public string? PathCartaRecomendacion { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Estado de la Solicitud")]
    public string EstadoSolicitud { get; set; } = "Recibida";

    // Campos específicos de Pasantía
    [StringLength(255)]
    [Display(Name = "Nombre de Universidad (si aplica)")]
    public string? NombreUniversidad { get; set; }

    [StringLength(255)]
    [Display(Name = "Carrera / Área de Estudio")]
    public string? CarreraAreaEstudio { get; set; }

    [Column(TypeName = "DATE")]
    [Display(Name = "Fecha de Graduación (o esperada)")]
    [DataType(DataType.Date)]
    public DateTime? FechaGraduacionEsperada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Preferencias de Alojamiento en Familia", Prompt = "Describe tus preferencias (ej: niños, animales, rural, urbano) y cualquier alergia alimentaria o de otro tipo que tu familia anfitriona deba conocer.")]
    [DataType(DataType.MultilineText)]
    public string? PreferenciasAlojamientoFamilia { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ensayo: Relación con Estudios/Intereses", Prompt = "Explica cómo esta pasantía se relaciona con tus estudios académicos, aspiraciones profesionales o intereses personales.")]
    [DataType(DataType.MultilineText)]
    public string? EnsayoRelacionEstudiosIntereses { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ensayo: Habilidades/Conocimientos a Adquirir", Prompt = "¿Qué habilidades o conocimientos específicos esperas obtener de esta experiencia?")]
    [DataType(DataType.MultilineText)]
    public string? EnsayoHabilidadesConocimientosAdquirir { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ensayo: Contribución y Aprendizaje en Comunidad Anfitriona", Prompt = "¿Cómo visualizas tu contribución y aprendizaje en la comunidad anfitriona durante tu programa de pasantía?")]
    [DataType(DataType.MultilineText)]
    public string? EnsayoContribucionAprendizajeComunidad { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Ensayo: Experiencias Transculturales Previas", Prompt = "Discute cualquier experiencia transcultural previa que te haya preparado para vivir y trabajar en un entorno diverso como Costa Rica.")]
    [DataType(DataType.MultilineText)]
    public string? EnsayoExperienciasTransculturalesPrevias { get; set; }

    // Campos de Contacto de Emergencia
    [StringLength(200)]
    [Display(Name = "Nombre Contacto de Emergencia")]
    public string? NombreContactoEmergencia { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono Contacto de Emergencia")]
    public string? TelefonoContactoEmergencia { get; set; }

    [StringLength(254)]
    [EmailAddress]
    [Display(Name = "Email Contacto de Emergencia")]
    public string? EmailContactoEmergencia { get; set; }

    [StringLength(50)]
    [Display(Name = "Relación Contacto de Emergencia")]
    public string? RelacionContactoEmergencia { get; set; }

    // Campos de Dominio del Español
    [StringLength(100)]
    [Display(Name = "Años de Entrenamiento Formal en Español")]
    public string? AniosEntrenamientoFormalEsp { get; set; }

    [Display(Name = "Comodidad con Habilidades en Español (1-5)")]
    [Range(1, 5, ErrorMessage = "El valor debe estar entre 1 y 5.")] // AÑADIDO
    public int? ComodidadHabilidadesEsp { get; set; }

    // Campos Generales Adicionales
    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Información Personal (familia, hobbies, gustos/disgustos)", Prompt = "Cuéntanos un poco sobre ti.")]
    [DataType(DataType.MultilineText)]
    public string? InfoPersonalFamiliaHobbies { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Alergias o Restricciones Dietéticas (especificar)")]
    [DataType(DataType.MultilineText)]
    public string? AlergiasRestriccionesDieteticas { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Solicitudes Especiales para Alojamiento")]
    [DataType(DataType.MultilineText)]
    public string? SolicitudesEspecialesAlojamiento { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("NivelIdiomaEspañolID")]
    [Display(Name = "Nivel de Español")]
    public virtual NivelesIdioma? NivelesIdioma { get; set; }

    [ForeignKey("FuenteConocimientoID")]
    [Display(Name = "¿Cómo nos conoció?")]
    public virtual FuentesConocimiento? FuentesConocimiento { get; set; }

    public virtual ICollection<SolicitudCamposInteres> SolicitudCamposInteres { get; set; } = new List<SolicitudCamposInteres>();
    public virtual ICollection<ParticipacionesActivas> ParticipacionesActivas { get; set; } = new List<ParticipacionesActivas>();
  }
}
