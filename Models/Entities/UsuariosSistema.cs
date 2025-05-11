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

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre de Usuario (Login)")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(255)] // La longitud dependerá de cómo almacenes el hash
    [Display(Name = "Contraseña")]
    [DataType(DataType.Password)] // Para que el input sea de tipo password
    public string HashContrasena { get; set; } = null!;

    // Podrías añadir un campo [NotMapped] para confirmar contraseña en el ViewModel si lo deseas,
    // pero no en la entidad de base de datos.

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico no válido.")]
    [StringLength(254)]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    [Display(Name = "Rol del Usuario")]
    public int RolUsuarioID { get; set; } // FK

    [Display(Name = "Usuario Activo")]
    public bool Activo { get; set; } = true;

    [Display(Name = "Último Acceso")]
    [DataType(DataType.DateTime)]
    public DateTime? FechaUltimoAcceso { get; set; }

    // --- Propiedades de Navegación ---
    [ForeignKey("RolUsuarioID")]
    [Display(Name = "Rol")]
    public virtual RolesSistema RolesSistema { get; set; } = null!;

    public virtual ICollection<ProgramasProyectosONG> ProgramasProyectosONGResponsable { get; set; } = new List<ProgramasProyectosONG>();
    public virtual ICollection<SolicitudesInformacionGeneral> SolicitudesInformacionGeneralAsignadas { get; set; } = new List<SolicitudesInformacionGeneral>();

    [NotMapped] // Para que EF Core no intente mapearla a la BD
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto
    {
      get { return $"{Nombres} {Apellidos}"; }
    }
  }
}
