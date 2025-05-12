using Microsoft.AspNetCore.Identity; // Necesario para IdentityUserClaim, etc.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Necesario para IdentityDbContext
using Microsoft.EntityFrameworkCore;
using VN_Center.Models.Entities;

namespace VN_Center.Data
{
  // Heredamos de IdentityDbContext, especificando nuestras clases personalizadas para Usuario y Rol,
  // y el tipo de clave primaria (int). Los otros tipos genéricos son para las tablas de Identity.
  public class VNCenterDbContext : IdentityDbContext<
      UsuariosSistema, RolesSistema, int,
      IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
      IdentityRoleClaim<int>, IdentityUserToken<int>>
  {
    public VNCenterDbContext(DbContextOptions<VNCenterDbContext> options)
        : base(options)
    {
    }

    // Ya no necesitamos DbSet para UsuariosSistema y RolesSistema,
    // IdentityDbContext los proporciona como Users y Roles respectivamente.
    // public DbSet<UsuariosSistema> UsuariosSistema { get; set; } = null!; // Removido
    // public DbSet<RolesSistema> RolesSistema { get; set; } = null!;       // Removido

    // Mantener los DbSet para tus otras entidades personalizadas
    public DbSet<NivelesIdioma> NivelesIdioma { get; set; } = null!;
    public DbSet<FuentesConocimiento> FuentesConocimiento { get; set; } = null!;
    public DbSet<CamposInteresVocacional> CamposInteresVocacional { get; set; } = null!;
    public DbSet<TiposAsistencia> TiposAsistencia { get; set; } = null!;
    public DbSet<Permisos> Permisos { get; set; } = null!; // Permisos sigue siendo una entidad personalizada
    public DbSet<Comunidades> Comunidades { get; set; } = null!;
    public DbSet<GruposComunitarios> GruposComunitarios { get; set; } = null!;
    public DbSet<ProgramasProyectosONG> ProgramasProyectosONG { get; set; } = null!;
    public DbSet<Solicitudes> Solicitudes { get; set; } = null!;
    public DbSet<Beneficiarios> Beneficiarios { get; set; } = null!;
    public DbSet<SolicitudesInformacionGeneral> SolicitudesInformacionGeneral { get; set; } = null!;
    public DbSet<RolPermisos> RolPermisos { get; set; } = null!; // Tabla de cruce personalizada
    public DbSet<SolicitudCamposInteres> SolicitudCamposInteres { get; set; } = null!;
    public DbSet<BeneficiarioAsistenciaRecibida> BeneficiarioAsistenciaRecibida { get; set; } = null!;
    public DbSet<BeneficiarioGrupos> BeneficiarioGrupos { get; set; } = null!;
    public DbSet<ProgramaProyectoComunidades> ProgramaProyectoComunidades { get; set; } = null!;
    public DbSet<ProgramaProyectoGrupos> ProgramaProyectoGrupos { get; set; } = null!;
    public DbSet<ParticipacionesActivas> ParticipacionesActivas { get; set; } = null!;
    public DbSet<BeneficiariosProgramasProyectos> BeneficiariosProgramasProyectos { get; set; } = null!;
    public DbSet<EvaluacionesPrograma> EvaluacionesPrograma { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // MUY IMPORTANTE: Llamar a base.OnModelCreating para Identity

      // Identity configura sus propias tablas (AspNetUsers, AspNetRoles, etc.).
      // Aquí solo necesitamos configurar nuestras tablas personalizadas y sus relaciones,
      // especialmente las claves primarias compuestas para las tablas de cruce.

      modelBuilder.Entity<RolPermisos>()
          .HasKey(rp => new { rp.RolUsuarioID, rp.PermisoID });
      // Asegurar que las FK en RolPermisos apunten a las tablas de Identity (AspNetRoles)
      modelBuilder.Entity<RolPermisos>()
          .HasOne(rp => rp.RolesSistema)
          .WithMany(r => r.RolPermisos)
          .HasForeignKey(rp => rp.RolUsuarioID);
      modelBuilder.Entity<RolPermisos>()
          .HasOne(rp => rp.Permisos)
          .WithMany(p => p.RolPermisos)
          .HasForeignKey(rp => rp.PermisoID);


      modelBuilder.Entity<SolicitudCamposInteres>()
          .HasKey(sci => new { sci.SolicitudID, sci.CampoInteresID });

      modelBuilder.Entity<BeneficiarioAsistenciaRecibida>()
          .HasKey(bar => new { bar.BeneficiarioID, bar.TipoAsistenciaID });

      modelBuilder.Entity<BeneficiarioGrupos>()
          .HasKey(bg => new { bg.BeneficiarioID, bg.GrupoID });

      modelBuilder.Entity<ProgramaProyectoComunidades>()
          .HasKey(ppc => new { ppc.ProgramaProyectoID, ppc.ComunidadID });

      modelBuilder.Entity<ProgramaProyectoGrupos>()
          .HasKey(ppg => new { ppg.ProgramaProyectoID, ppg.GrupoID });

      modelBuilder.Entity<BeneficiariosProgramasProyectos>()
          .HasKey(bpp => new { bpp.BeneficiarioID, bpp.ProgramaProyectoID });

      // Cambiar el nombre de las tablas de Identity si quieres que coincidan con tus nombres originales
      // (Opcional, pero puede ayudar a mantener la consistencia si ya tienes datos o scripts)
      // modelBuilder.Entity<UsuariosSistema>(b =>
      // {
      //     b.ToTable("UsuariosSistema"); // Mapea a tu tabla original
      // });
      // modelBuilder.Entity<RolesSistema>(b =>
      // {
      //     b.ToTable("RolesSistema"); // Mapea a tu tabla original
      // });
      // Y así para las otras tablas de Identity (AspNetUserRoles, AspNetUserClaims, etc.) si es necesario.
      // Sin embargo, es más común dejar que Identity use sus nombres por defecto (AspNetUsers, AspNetRoles).
    }
  }
}
