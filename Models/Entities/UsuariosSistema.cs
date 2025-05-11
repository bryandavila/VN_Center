using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VN_Center.Models.Entities
{
  [Table("UsuariosSistema")]
  public class UsuariosSistema
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UsuarioID { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreUsuario { get; set; } = null!;

    [NotMapped] // Para que EF Core no intente mapearla a la BD
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto
    {
      get { return $"{Nombres} {Apellidos}"; }
    }

    [Required]
    [StringLength(255)] // Ajusta la longitud según cómo almacenes el hash
    public string HashContrasena { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Nombres { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Apellidos { get; set; } = null!;

    [Required]
    [StringLength(254)]
    public string Email { get; set; } = null!;

    public int RolUsuarioID { get; set; } // FK

    public bool Activo { get; set; } = true;

    public DateTime? FechaUltimoAcceso { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("RolUsuarioID")]
    public virtual RolesSistema RolesSistema { get; set; } = null!;

    // Un usuario puede ser responsable de muchos ProgramasProyectosONG
    public virtual ICollection<ProgramasProyectosONG> ProgramasProyectosONGResponsable { get; set; } = new List<ProgramasProyectosONG>();

    // Un usuario puede estar asignado a muchas SolicitudesInformacionGeneral
    public virtual ICollection<SolicitudesInformacionGeneral> SolicitudesInformacionGeneralAsignadas { get; set; } = new List<SolicitudesInformacionGeneral>();
  }
}
