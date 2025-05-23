// VN_Center/Models/Entities/Beneficiarios.cs
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
    public string? RangoEdad { get; set; }

    [StringLength(20)]
    [Display(Name = "Género")]
    public string? Genero { get; set; }

    [StringLength(100)]
    [Display(Name = "País de Origen")]
    public string? PaisOrigen { get; set; }

    [StringLength(100)]
    [Display(Name = "Otro País de Origen (especificar)")]
    public string? OtroPaisOrigen { get; set; }

    [StringLength(50)]
    [Display(Name = "Estado Migratorio")]
    public string? EstadoMigratorio { get; set; }

    [StringLength(100)]
    [Display(Name = "Otro Estado Migratorio (especificar)")]
    public string? OtroEstadoMigratorio { get; set; }

    [Display(Name = "Número de Personas en el Hogar")]
    public int? NumeroPersonasHogar { get; set; }

    [StringLength(20)]
    [Display(Name = "Vivienda (Alquila/Propia)")]
    public string? ViviendaAlquiladaOPropia { get; set; }

    [Display(Name = "Miembros del Hogar Empleados")]
    public int? MiembrosHogarEmpleados { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Está Empleado Personalmente?")]
    public string? EstaEmpleadoPersonalmente { get; set; }

    [StringLength(100)]
    [Display(Name = "Tipo de Situación Laboral")]
    public string? TipoSituacionLaboral { get; set; }

    [StringLength(100)]
    [Display(Name = "Otra Situación Laboral (especificar)")]
    public string? OtroTipoSituacionLaboral { get; set; }

    [StringLength(100)]
    [Display(Name = "Tipo de Trabajo Realizado (si empleado)")]
    public string? TipoTrabajoRealizadoSiEmpleado { get; set; }

    [StringLength(100)]
    [Display(Name = "Otro Tipo de Trabajo (especificar)")]
    public string? OtroTipoTrabajoRealizado { get; set; }

    [StringLength(30)]
    [Display(Name = "Estado Civil")]
    public string? EstadoCivil { get; set; }

    [StringLength(50)]
    [Display(Name = "Tiempo en Costa Rica (si migrante)")]
    public string? TiempoEnCostaRicaSiMigrante { get; set; }

    [StringLength(50)]
    [Display(Name = "Tiempo Viviendo en Comunidad Actual")]
    public string? TiempoViviendoEnComunidadActual { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Ingresos Suficientes para Necesidades?")]
    public string? IngresosSuficientesNecesidades { get; set; }

    [StringLength(100)]
    [Display(Name = "Nivel Máximo de Educación Completado")]
    public string? NivelEducacionCompletado { get; set; }

    [StringLength(100)]
    [Display(Name = "Otro Nivel de Educación (especificar)")]
    public string? OtroNivelEducacion { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Inscrito en Programa de Educación/Capacitación?")]
    public string? InscritoProgramaEducacionCapacitacionActual { get; set; }

    [StringLength(30)]
    [Display(Name = "¿Niños del Hogar Asisten a Escuela?")]
    public string? NinosHogarAsistenEscuela { get; set; }

    [Column(TypeName = "NVARCHAR(MAX)")]
    [Display(Name = "Barreras Asistencia Escolar Niños", Prompt = "Si indicó 'No', ¿cuáles son las barreras? (Costo, Documentación, Distancia, Otros)")]
    [DataType(DataType.MultilineText)]
    public string? BarrerasAsistenciaEscolarNinos { get; set; }

    [StringLength(255)]
    [Display(Name = "Otras Barreras Asistencia Escolar (especificar)")]
    public string? OtroBarrerasAsistenciaEscolar { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Mujeres con Acceso Igual a Oportunidades Laborales?")]
    public string? PercepcionAccesoIgualOportunidadesLaboralesMujeres { get; set; }

    [StringLength(10)]
    [Display(Name = "¿Servicios Disponibles para Mujeres Víctimas de Violencia?")]
    public string? DisponibilidadServiciosMujeresVictimasViolencia { get; set; }

    [StringLength(10)]
    [Display(Name = "¿Servicios de Salud para la Mujer Disponibles?")]
    public string? DisponibilidadServiciosSaludMujer { get; set; }

    [StringLength(10)]
    [Display(Name = "¿Servicios de Apoyo para Adultos Mayores Disponibles?")]
    public string? DisponibilidadServiciosApoyoAdultosMayores { get; set; }

    [StringLength(5)]
    [Display(Name = "¿Servicios de Transporte Accesibles?")]
    public string? AccesibilidadServiciosTransporteComunidad { get; set; }

    [StringLength(20)]
    [Display(Name = "Acceso a Computadora")]
    public string? AccesoComputadora { get; set; }

    [StringLength(20)]
    [Display(Name = "Acceso a Internet")]
    public string? AccesoInternet { get; set; }

    // --- NUEVA PROPIEDAD PARA AUDITORÍA Y FILTRADO ---
    [StringLength(450)]
    [Display(Name = "Usuario Creador ID")]
    public string? UsuarioCreadorId { get; set; }
    // --- FIN DE NUEVA PROPIEDAD ---

    // --- Propiedades de Navegación ---
    [ForeignKey("ComunidadID")]
    [Display(Name = "Comunidad")]
    public virtual Comunidades? Comunidad { get; set; }

    public virtual ICollection<BeneficiarioAsistenciaRecibida> BeneficiarioAsistenciaRecibida { get; set; } = new List<BeneficiarioAsistenciaRecibida>();
    public virtual ICollection<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = new List<BeneficiarioGrupos>();
    public virtual ICollection<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = new List<BeneficiariosProgramasProyectos>();

    // --- Propiedad Calculada (No Mapeada) ---
    [NotMapped]
    [Display(Name = "Nombre Completo del Beneficiario")]
    public string NombreCompleto
    {
      get
      {
        if (string.IsNullOrWhiteSpace(Nombres) && string.IsNullOrWhiteSpace(Apellidos))
        {
          return "Beneficiario (ID: " + BeneficiarioID + ")";
        }
        return $"{Nombres} {Apellidos}".Trim();
      }
    }
  }
}
