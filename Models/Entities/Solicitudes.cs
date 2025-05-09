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
    public string Nombres { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Apellidos { get; set; } = null!;

    [Required]
    [StringLength(254)]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string? Telefono { get; set; }

    public bool? PermiteContactoWhatsApp { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? Direccion { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaNacimiento { get; set; }

    [Required]
    [StringLength(20)]
    public string TipoSolicitud { get; set; } = null!; // "Voluntariado", "Pasantia"

    public DateTime FechaEnvioSolicitud { get; set; } = DateTime.UtcNow;

    public bool? PasaporteValidoSeisMeses { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaExpiracionPasaporte { get; set; }

    [StringLength(255)]
    public string? ProfesionOcupacion { get; set; }

    public int? NivelIdiomaEspañolID { get; set; } // FK

    [StringLength(255)]
    public string? OtrosIdiomasHablados { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ExperienciaPreviaSVL { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? ExperienciaLaboralRelevante { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? HabilidadesRelevantes { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaInicioPreferida { get; set; }

    [StringLength(100)]
    public string? DuracionEstancia { get; set; }

    [StringLength(100)]
    public string? DuracionEstanciaOtro { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? MotivacionInteresCR { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? DescripcionSalidaZonaConfort { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? InformacionAdicionalPersonal { get; set; }

    public int? FuenteConocimientoID { get; set; } // FK

    [StringLength(255)]
    public string? FuenteConocimientoOtro { get; set; }

    [StringLength(512)]
    public string? PathCV { get; set; }

    [StringLength(512)]
    public string? PathCartaRecomendacion { get; set; }

    [Required]
    [StringLength(50)]
    public string EstadoSolicitud { get; set; } = "Recibida";

    // Campos específicos de Pasantía
    [StringLength(255)]
    public string? NombreUniversidad { get; set; }

    [StringLength(255)]
    public string? CarreraAreaEstudio { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? FechaGraduacionEsperada { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? PreferenciasAlojamientoFamilia { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? EnsayoRelacionEstudiosIntereses { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? EnsayoHabilidadesConocimientosAdquirir { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? EnsayoContribucionAprendizajeComunidad { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? EnsayoExperienciasTransculturalesPrevias { get; set; }

    // Campos de Contacto de Emergencia
    [StringLength(200)]
    public string? NombreContactoEmergencia { get; set; }

    [StringLength(30)]
    public string? TelefonoContactoEmergencia { get; set; }

    [StringLength(254)]
    public string? EmailContactoEmergencia { get; set; }

    [StringLength(50)]
    public string? RelacionContactoEmergencia { get; set; }

    // Campos de Dominio del Español
    [StringLength(100)]
    public string? AniosEntrenamientoFormalEsp { get; set; }
    public int? ComodidadHabilidadesEsp { get; set; } // Escala 1-5

    // Campos Generales Adicionales
    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? InfoPersonalFamiliaHobbies { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? AlergiasRestriccionesDieteticas { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? SolicitudesEspecialesAlojamiento { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("NivelIdiomaEspañolID")]
    public virtual NivelesIdioma? NivelesIdioma { get; set; }

    [ForeignKey("FuenteConocimientoID")]
    public virtual FuentesConocimiento? FuentesConocimiento { get; set; }

    // Relación muchos-a-muchos con CamposInteresVocacional (a través de SolicitudCamposInteres)
    public virtual ICollection<SolicitudCamposInteres> SolicitudCamposInteres { get; set; } = new List<SolicitudCamposInteres>();

    // Relación uno-a-muchos con ParticipacionesActivas (una solicitud puede tener muchas participaciones)
    public virtual ICollection<ParticipacionesActivas> ParticipacionesActivas { get; set; } = new List<ParticipacionesActivas>();
  }
}
