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

    public DateTime FechaRegistroBeneficiario { get; set; } = DateTime.UtcNow;

    public int? ComunidadID { get; set; } // FK

    [StringLength(100)]
    public string? Nombres { get; set; }

    [StringLength(100)]
    public string? Apellidos { get; set; }

    [StringLength(20)]
    public string? RangoEdad { get; set; }

    [StringLength(20)]
    public string? Genero { get; set; }

    [StringLength(100)]
    public string? PaisOrigen { get; set; }

    [StringLength(100)]
    public string? OtroPaisOrigen { get; set; }

    [StringLength(50)]
    public string? EstadoMigratorio { get; set; }

    [StringLength(100)]
    public string? OtroEstadoMigratorio { get; set; }

    public int? NumeroPersonasHogar { get; set; }

    [StringLength(20)]
    public string? ViviendaAlquiladaOPropia { get; set; }

    public int? MiembrosHogarEmpleados { get; set; }

    [StringLength(5)]
    public string? EstaEmpleadoPersonalmente { get; set; } // "Sí", "No"

    [StringLength(100)]
    public string? TipoSituacionLaboral { get; set; }

    [StringLength(100)]
    public string? OtroTipoSituacionLaboral { get; set; }

    [StringLength(100)]
    public string? TipoTrabajoRealizadoSiEmpleado { get; set; }

    [StringLength(100)]
    public string? OtroTipoTrabajoRealizado { get; set; }

    [StringLength(30)]
    public string? EstadoCivil { get; set; }

    [StringLength(50)]
    public string? TiempoEnCostaRicaSiMigrante { get; set; }

    [StringLength(50)]
    public string? TiempoViviendoEnComunidadActual { get; set; }

    [StringLength(5)]
    public string? IngresosSuficientesNecesidades { get; set; } // "Sí", "No"

    [StringLength(100)]
    public string? NivelEducacionCompletado { get; set; }

    [StringLength(100)]
    public string? OtroNivelEducacion { get; set; }

    [StringLength(5)]
    public string? InscritoProgramaEducacionCapacitacionActual { get; set; } // "Sí", "No"

    [StringLength(30)]
    public string? NinosHogarAsistenEscuela { get; set; } // "Sí", "No", "No hay niños..."

    [Column(TypeName = "NVARCHAR(MAX)")]
    public string? BarrerasAsistenciaEscolarNinos { get; set; }

    [StringLength(255)]
    public string? OtroBarrerasAsistenciaEscolar { get; set; }

    [StringLength(5)]
    public string? PercepcionAccesoIgualOportunidadesLaboralesMujeres { get; set; } // "Sí", "No"

    [StringLength(10)]
    public string? DisponibilidadServiciosMujeresVictimasViolencia { get; set; } // "Sí", "No", "No sé"

    [StringLength(10)]
    public string? DisponibilidadServiciosSaludMujer { get; set; } // "Sí", "No", "No sé"

    [StringLength(10)]
    public string? DisponibilidadServiciosApoyoAdultosMayores { get; set; } // "Sí", "No", "No sé"

    [StringLength(5)]
    public string? AccesibilidadServiciosTransporteComunidad { get; set; } // "Sí", "No"

    [StringLength(20)]
    public string? AccesoComputadora { get; set; } // "Sí", "No", "A veces"

    [StringLength(20)]
    public string? AccesoInternet { get; set; } // "Sí", "No", "A veces"

    // --- Propiedades de Navegación ---
    [ForeignKey("ComunidadID")]
    public virtual Comunidades? Comunidad { get; set; }

    // Relación muchos-a-muchos con TiposAsistencia (a través de BeneficiarioAsistenciaRecibida)
    public virtual ICollection<BeneficiarioAsistenciaRecibida> BeneficiarioAsistenciaRecibida { get; set; } = new List<BeneficiarioAsistenciaRecibida>();

    // Relación muchos-a-muchos con GruposComunitarios (a través de BeneficiarioGrupos)
    public virtual ICollection<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = new List<BeneficiarioGrupos>();

    // Relación muchos-a-muchos con ProgramasProyectosONG (a través de BeneficiariosProgramasProyectos)
    public virtual ICollection<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = new List<BeneficiariosProgramasProyectos>();
  }
}
