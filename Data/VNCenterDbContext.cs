using Microsoft.EntityFrameworkCore;
using VN_Center.Models.Entities;

namespace VN_Center.Data
{
  public class VNCenterDbContext : DbContext
  {
    public VNCenterDbContext(DbContextOptions<VNCenterDbContext> options)
        : base(options)
    {
    }

    // Tablas de Catálogo y Entidades Independientes
    public DbSet<NivelesIdioma> NivelesIdioma { get; set; } = null!;
    public DbSet<FuentesConocimiento> FuentesConocimiento { get; set; } = null!;
    public DbSet<CamposInteresVocacional> CamposInteresVocacional { get; set; } = null!;
    public DbSet<TiposAsistencia> TiposAsistencia { get; set; } = null!;
    public DbSet<RolesSistema> RolesSistema { get; set; } = null!;
    public DbSet<Permisos> Permisos { get; set; } = null!;
    public DbSet<Comunidades> Comunidades { get; set; } = null!;

    // Entidades Principales
    public DbSet<UsuariosSistema> UsuariosSistema { get; set; } = null!;
    public DbSet<GruposComunitarios> GruposComunitarios { get; set; } = null!;
    public DbSet<ProgramasProyectosONG> ProgramasProyectosONG { get; set; } = null!;
    public DbSet<Solicitudes> Solicitudes { get; set; } = null!;
    public DbSet<Beneficiarios> Beneficiarios { get; set; } = null!;
    public DbSet<SolicitudesInformacionGeneral> SolicitudesInformacionGeneral { get; set; } = null!;

    // Tablas de Cruce y Entidades Dependientes
    public DbSet<RolPermisos> RolPermisos { get; set; } = null!;
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
      base.OnModelCreating(modelBuilder);

      // Configuración de claves primarias compuestas para tablas de cruce
      // EF Core a menudo puede inferirlas si las propiedades FK están bien definidas,
      // pero es bueno ser explícito.
      modelBuilder.Entity<RolPermisos>()
          .HasKey(rp => new { rp.RolUsuarioID, rp.PermisoID });

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

      // Aquí puedes añadir configuraciones adicionales con Fluent API si son necesarias.
      // Por ejemplo, para las relaciones ON DELETE CASCADE que definiste en SQL,
      // EF Core tiene convenciones, pero puedes ser explícito:
      // modelBuilder.Entity<SolicitudCamposInteres>()
      //     .HasOne(sci => sci.Solicitudes)
      //     .WithMany(s => s.SolicitudCamposInteres)
      //     .HasForeignKey(sci => sci.SolicitudID)
      //     .OnDelete(DeleteBehavior.Cascade); // Configura ON DELETE CASCADE

      // Repite para otras relaciones con ON DELETE CASCADE según tu script SQL.
    }
  }
}
