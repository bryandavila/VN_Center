using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("Beneficiarios")]
  public class Beneficiarios
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BeneficiarioID { get; set; }

    [Display(Name = "Fecha de Registro")]
    [DataType(DataType.Date)]
    public DateTime FechaRegistroBeneficiario { get; set; } = DateTime.UtcNow;

    [Display(Name = "Comunidad")]
    public int? ComunidadID { get; set; } // FK

    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string? Nombres { get; set; }

    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string? Apellidos { get; set; }

    [StringLength(20)]
    [Display(Name = "Rango de Edad")]
    public string? RangoEdad { get; set; } // Considerar un dropdown si los rangos son fijos

    [StringLength(20)]
    [Display(Name = "Género")]
    public string? Genero { get; set; } // Considerar un dropdown

    [StringLength(100)]
    [Display(Name = "País de Origen")]
    public string? PaisOrigen { get; set; }

    [StringLength(100)]
    [Display(Name = "Otro País de Origen (especificar)")]
    public string? OtroPaisOrigen { get; set; }

    [StringLength(50)]
    [Display(Name = "Estado Migratorio")]
    public string? EstadoMigratorio { get; set; } // Considerar un dropdown

    [StringLength(100)]
    [Display(Name = "Otro Estado Migratorio (especificar)")]
    public string? OtroEstadoMigratorio { get; set; }

    [Display(Name = "Número de Personas en el Hogar")]
    public int? NumeroPersonasHogar { get; set; }

    [StringLength(20)]
    [Display(Name = "Vivienda (Alquila/Propia)")]
    public string? ViviendaAlquiladaOPropia { get; set; } // Considerar dropdown (Alquilo, Propietario)

    [Display(Name = "Miembros del Hogar Empleados")]
    public int? MiembrosHogarEmpleados { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Está Empleado Personalmente?")]
    public string? EstaEmpleadoPersonalmente { get; set; } // Considerar dropdown (Sí, No)

    [StringLength(100)]
    [Display(Name = "Tipo de Situación Laboral")]
    public string? TipoSituacionLaboral { get; set; } // Considerar dropdown

    [StringLength(100)]
    [Display(Name = "Otra Situación Laboral (especificar)")]
    public string? OtroTipoSituacionLaboral { get; set; }

    [StringLength(100)]
    [Display(Name = "Tipo de Trabajo Realizado (si empleado)")]
    public string? TipoTrabajoRealizadoSiEmpleado { get; set; } // Considerar dropdown

    [StringLength(100)]
    [Display(Name = "Otro Tipo de Trabajo (especificar)")]
    public string? OtroTipoTrabajoRealizado { get; set; }

    [StringLength(30)]
    [Display(Name = "Estado Civil")]
    public string? EstadoCivil { get; set; } // Considerar dropdown

    [StringLength(50)]
    [Display(Name = "Tiempo en Costa Rica (si migrante)")]
    public string? TiempoEnCostaRicaSiMigrante { get; set; }

    [StringLength(50)]
    [Display(Name = "Tiempo Viviendo en Comunidad Actual")]
    public string? TiempoViviendoEnComunidadActual { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Ingresos Suficientes para Necesidades?")]
    public string? IngresosSuficientesNecesidades { get; set; } // Considerar dropdown (Sí, No)

    [StringLength(100)]
    [Display(Name = "Nivel Máximo de Educación Completado")]
    public string? NivelEducacionCompletado { get; set; } // Considerar dropdown

    [StringLength(100)]
    [Display(Name = "Otro Nivel de Educación (especificar)")]
    public string? OtroNivelEducacion { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Inscrito en Programa de Educación/Capacitación?")]
    public string? InscritoProgramaEducacionCapacitacionActual { get; set; } // Considerar dropdown (Sí, No)

    [StringLength(30)]
    [Display(Name = "¿Niños del Hogar Asisten a Escuela?")]
    public string? NinosHogarAsistenEscuela { get; set; } // Considerar dropdown (Sí, No, No hay niños)

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Barreras Asistencia Escolar Niños", Prompt = "Si indicó 'No', ¿cuáles son las barreras? (Costo, Documentación, Distancia, Otros)")]
    [DataType(DataType.MultilineText)]
    public string? BarrerasAsistenciaEscolarNinos { get; set; }

    [StringLength(255)]
    [Display(Name = "Otras Barreras Asistencia Escolar (especificar)")]
    public string? OtroBarrerasAsistenciaEscolar { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Mujeres con Acceso Igual a Oportunidades Laborales?")]
    public string? PercepcionAccesoIgualOportunidadesLaboralesMujeres { get; set; } // Considerar dropdown (Sí, No)

    [StringLength(10)]
    [Display(Name = "¿Servicios Disponibles para Mujeres Víctimas de Violencia?")]
    public string? DisponibilidadServiciosMujeresVictimasViolencia { get; set; } // Considerar dropdown (Sí, No, No sé)

    [StringLength(10)]
    [Display(Name = "¿Servicios de Salud para la Mujer Disponibles?")]
    public string? DisponibilidadServiciosSaludMujer { get; set; } // Considerar dropdown (Sí, No, No sé)

    [StringLength(10)]
    [Display(Name = "¿Servicios de Apoyo para Adultos Mayores Disponibles?")]
    public string? DisponibilidadServiciosApoyoAdultosMayores { get; set; } // Considerar dropdown (Sí, No, No sé)

    [StringLength(5)]
    [Display(Name = "¿Servicios de Transporte Accesibles?")]
    public string? AccesibilidadServiciosTransporteComunidad { get; set; } // Considerar dropdown (Sí, No)

    [StringLength(20)]
    [Display(Name = "Acceso a Computadora")]
    public string? AccesoComputadora { get; set; } // Considerar dropdown (Sí, No, A veces)

    [StringLength(20)]
    [Display(Name = "Acceso a Internet")]
    public string? AccesoInternet { get; set; } // Considerar dropdown (Sí, No, A veces)

    // --- Propiedades de Navegación ---
    [ForeignKey("ComunidadID")]
    [Display(Name = "Comunidad")]
    public virtual Comunidades? Comunidad { get; set; }

    public virtual ICollection<BeneficiarioAsistenciaRecibida> BeneficiarioAsistenciaRecibida { get; set; } = new List<BeneficiarioAsistenciaRecibida>();
    public virtual ICollection<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = new List<BeneficiarioGrupos>();
    public virtual ICollection<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = new List<BeneficiariosProgramasProyectos>();
  }
}
