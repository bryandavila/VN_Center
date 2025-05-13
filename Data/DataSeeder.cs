using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VN_Center.Models.Entities; // Asegúrate que este es el namespace correcto para tus entidades
using System.Linq; // Necesario para .Any() y otros métodos LINQ

namespace VN_Center.Data // O el namespace que prefieras para esta clase
{
  public static class DataSeeder
  {
    public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<VNCenterDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuariosSistema>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RolesSistema>>();

        // Aplicar migraciones pendientes (asegura que la BD esté al día antes de sembrar)
        // Comentado si prefieres hacerlo manualmente con Update-Database
        // await context.Database.MigrateAsync(); 

        // Sembrar datos de catálogo
        await SeedNivelesIdiomaAsync(context);
        await SeedFuentesConocimientoAsync(context);
        await SeedCamposInteresVocacionalAsync(context);
        await SeedTiposAsistenciaAsync(context);
        await SeedComunidadesAsync(context); // Asumiendo que Comunidades es una tabla de catálogo

        // Sembrar Permisos (antes de Roles y RolPermisos si estos dependen de Permisos)
        await SeedPermisosAsync(context);

        // Sembrar Roles de Identity
        await SeedRolesAsync(roleManager);

        // Sembrar Usuario Administrador de Identity
        // Obtén los datos del admin desde appsettings.json para no hardcodearlos
        var adminEmail = configuration["AppSettings:AdminUser:Email"] ?? "admin@vncenter.org";
        var adminPassword = configuration["AppSettings:AdminUser:Password"] ?? "Admin123!"; // ¡Cambia esto en producción!
        var adminNombres = configuration["AppSettings:AdminUser:Nombres"] ?? "Administrador";
        var adminApellidos = configuration["AppSettings:AdminUser:Apellidos"] ?? "Principal";

        await SeedAdminUserAsync(userManager, adminEmail, adminPassword, adminNombres, adminApellidos);

        // Sembrar RolPermisos (asignar permisos a roles)
        await SeedRolPermisosAsync(context, roleManager);
      }
    }

    private static async Task SeedNivelesIdiomaAsync(VNCenterDbContext context)
    {
      if (!await context.NivelesIdioma.AnyAsync())
      {
        var niveles = new List<NivelesIdioma>
                {
                    new NivelesIdioma { NombreNivel = "Beginner" },
                    new NivelesIdioma { NombreNivel = "Intermediate" },
                    new NivelesIdioma { NombreNivel = "Advanced" },
                    new NivelesIdioma { NombreNivel = "Other" }
                };
        await context.NivelesIdioma.AddRangeAsync(niveles);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para NivelesIdioma insertados.");
      }
    }

    private static async Task SeedFuentesConocimientoAsync(VNCenterDbContext context)
    {
      if (!await context.FuentesConocimiento.AnyAsync())
      {
        var fuentes = new List<FuentesConocimiento>
                {
                    new FuentesConocimiento { NombreFuente = "Instagram" },
                    new FuentesConocimiento { NombreFuente = "Tiktok" },
                    new FuentesConocimiento { NombreFuente = "Facebook" },
                    new FuentesConocimiento { NombreFuente = "GoAbroad" },
                    new FuentesConocimiento { NombreFuente = "Online search" },
                    new FuentesConocimiento { NombreFuente = "University" },
                    new FuentesConocimiento { NombreFuente = "VN Alumni" },
                    new FuentesConocimiento { NombreFuente = "Other" }
                };
        await context.FuentesConocimiento.AddRangeAsync(fuentes);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para FuentesConocimiento insertados.");
      }
    }

    private static async Task SeedCamposInteresVocacionalAsync(VNCenterDbContext context)
    {
      if (!await context.CamposInteresVocacional.AnyAsync())
      {
        var campos = new List<CamposInteresVocacional>
                {
                    new CamposInteresVocacional { NombreCampo = "Women's groups" },
                    new CamposInteresVocacional { NombreCampo = "Children's groups" },
                    new CamposInteresVocacional { NombreCampo = "Adolescent and youth groups" },
                    new CamposInteresVocacional { NombreCampo = "Seniors" },
                    new CamposInteresVocacional { NombreCampo = "Migrant and asylum seeker communities" },
                    new CamposInteresVocacional { NombreCampo = "Environment and conservation" },
                    new CamposInteresVocacional { NombreCampo = "Social work" },
                    new CamposInteresVocacional { NombreCampo = "Human rights" },
                    new CamposInteresVocacional { NombreCampo = "Language, education, and tutoring" },
                    new CamposInteresVocacional { NombreCampo = "Agriculture and food security" },
                    new CamposInteresVocacional { NombreCampo = "Health education" },
                    new CamposInteresVocacional { NombreCampo = "Small business development projects" },
                    new CamposInteresVocacional { NombreCampo = "Social media, communication, journalism, photography" },
                    new CamposInteresVocacional { NombreCampo = "Other" }
                };
        await context.CamposInteresVocacional.AddRangeAsync(campos);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para CamposInteresVocacional insertados.");
      }
    }

    private static async Task SeedTiposAsistenciaAsync(VNCenterDbContext context)
    {
      if (!await context.TiposAsistencia.AnyAsync())
      {
        var tipos = new List<TiposAsistencia>
                {
                    new TiposAsistencia { NombreAsistencia = "Alimentos" },
                    new TiposAsistencia { NombreAsistencia = "Vivienda" },
                    new TiposAsistencia { NombreAsistencia = "Educación" },
                    new TiposAsistencia { NombreAsistencia = "Salud" },
                    new TiposAsistencia { NombreAsistencia = "Empleo" },
                    new TiposAsistencia { NombreAsistencia = "No he recibido asistencia" }
                };
        await context.TiposAsistencia.AddRangeAsync(tipos);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para TiposAsistencia insertados.");
      }
    }

    private static async Task SeedComunidadesAsync(VNCenterDbContext context)
    {
      // Ejemplo, añade tus comunidades reales
      if (!await context.Comunidades.AnyAsync())
      {
        var comunidades = new List<Comunidades>
                {
                    new Comunidades { NombreComunidad = "Comunidad Ejemplo 1", UbicacionDetallada = "Cerca del centro" },
                    new Comunidades { NombreComunidad = "Comunidad Ejemplo 2", UbicacionDetallada = "Zona rural" }
                };
        await context.Comunidades.AddRangeAsync(comunidades);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para Comunidades insertados.");
      }
    }


    private static async Task SeedPermisosAsync(VNCenterDbContext context)
    {
      if (!await context.Permisos.AnyAsync())
      {
        var permisos = new List<Permisos>
                {
                    // Copia aquí los INSERT de tu script SQL, pero como objetos C#
                    // Ejemplo:
                    new Permisos { NombrePermiso = "Dashboard_Ver", DescripcionPermiso = "Ver el dashboard principal del sistema." },
                    new Permisos { NombrePermiso = "Solicitud_Crear", DescripcionPermiso = "Crear nuevas solicitudes de voluntarios/pasantes." },
                    new Permisos { NombrePermiso = "Solicitud_Listar", DescripcionPermiso = "Listar todas las solicitudes." },
                    new Permisos { NombrePermiso = "Solicitud_VerDetalle", DescripcionPermiso = "Ver el detalle completo de una solicitud." },
                    new Permisos { NombrePermiso = "Solicitud_ActualizarEstado", DescripcionPermiso = "Actualizar el estado de una solicitud (aprobada, rechazada, etc.)." },
                    new Permisos { NombrePermiso = "Solicitud_Editar", DescripcionPermiso = "Editar la información completa de una solicitud." },
                    new Permisos { NombrePermiso = "Solicitud_Eliminar", DescripcionPermiso = "Eliminar solicitudes." },
                    new Permisos { NombrePermiso = "Beneficiario_Crear", DescripcionPermiso = "Registrar nuevos beneficiarios." },
                    new Permisos { NombrePermiso = "Beneficiario_Listar", DescripcionPermiso = "Listar todos los beneficiarios." },
                    new Permisos { NombrePermiso = "Beneficiario_VerDetalle", DescripcionPermiso = "Ver el detalle completo de un beneficiario." },
                    new Permisos { NombrePermiso = "Beneficiario_Actualizar", DescripcionPermiso = "Actualizar información de beneficiarios." },
                    new Permisos { NombrePermiso = "Beneficiario_Eliminar", DescripcionPermiso = "Eliminar registros de beneficiarios." },
                    new Permisos { NombrePermiso = "ProgramaProyecto_Crear", DescripcionPermiso = "Crear nuevos programas o proyectos." },
                    new Permisos { NombrePermiso = "ProgramaProyecto_Listar", DescripcionPermiso = "Listar todos los programas y proyectos." },
                    new Permisos { NombrePermiso = "ProgramaProyecto_VerDetalle", DescripcionPermiso = "Ver el detalle de un programa/proyecto." },
                    new Permisos { NombrePermiso = "ProgramaProyecto_Actualizar", DescripcionPermiso = "Actualizar información de programas/proyectos." },
                    new Permisos { NombrePermiso = "ProgramaProyecto_Eliminar", DescripcionPermiso = "Eliminar programas o proyectos." },
                    new Permisos { NombrePermiso = "Comunidad_Gestionar", DescripcionPermiso = "Crear, editar, eliminar comunidades." },
                    new Permisos { NombrePermiso = "GrupoComunitario_Gestionar", DescripcionPermiso = "Crear, editar, eliminar grupos comunitarios." },
                    new Permisos { NombrePermiso = "Participacion_Asignar", DescripcionPermiso = "Asignar un voluntario/pasante a un programa/proyecto." },
                    new Permisos { NombrePermiso = "Participacion_Listar", DescripcionPermiso = "Listar participaciones activas." },
                    new Permisos { NombrePermiso = "Participacion_ActualizarDetalles", DescripcionPermiso = "Actualizar detalles de una participación (rol, horas, etc.)." },
                    new Permisos { NombrePermiso = "Participacion_Eliminar", DescripcionPermiso = "Eliminar una asignación de participación." },
                    new Permisos { NombrePermiso = "BeneficiarioPrograma_Asignar", DescripcionPermiso = "Inscribir un beneficiario en un programa/proyecto." },
                    new Permisos { NombrePermiso = "BeneficiarioPrograma_Listar", DescripcionPermiso = "Listar beneficiarios inscritos en programas." },
                    new Permisos { NombrePermiso = "BeneficiarioPrograma_ActualizarEstado", DescripcionPermiso = "Actualizar estado de participación de un beneficiario." },
                    new Permisos { NombrePermiso = "BeneficiarioPrograma_Eliminar", DescripcionPermiso = "Retirar un beneficiario de un programa." },
                    new Permisos { NombrePermiso = "Evaluacion_Registrar", DescripcionPermiso = "Permitir a un participante registrar su evaluación del programa." },
                    new Permisos { NombrePermiso = "Evaluacion_Listar", DescripcionPermiso = "Listar las evaluaciones de programas recibidas." },
                    new Permisos { NombrePermiso = "Evaluacion_VerDetalle", DescripcionPermiso = "Ver el detalle de una evaluación." },
                    new Permisos { NombrePermiso = "InfoGeneral_Listar", DescripcionPermiso = "Listar solicitudes de información general." },
                    new Permisos { NombrePermiso = "InfoGeneral_VerDetalle", DescripcionPermiso = "Ver detalle de solicitud de información." },
                    new Permisos { NombrePermiso = "InfoGeneral_ActualizarEstado", DescripcionPermiso = "Actualizar estado de solicitud de información (ej. respondida)." },
                    new Permisos { NombrePermiso = "Reporte_Generar", DescripcionPermiso = "Generar y ver reportes del sistema." },
                    new Permisos { NombrePermiso = "Usuario_Gestionar", DescripcionPermiso = "Crear, editar, eliminar usuarios del sistema." },
                    new Permisos { NombrePermiso = "RolPermiso_Gestionar", DescripcionPermiso = "Gestionar roles y asignar permisos a roles." },
                    new Permisos { NombrePermiso = "Sistema_Configurar", DescripcionPermiso = "Acceder a la configuración general del sistema." }
                };
        await context.Permisos.AddRangeAsync(permisos);
        await context.SaveChangesAsync();
        Console.WriteLine("Datos iniciales para Permisos insertados.");
      }
    }

    private static async Task SeedRolesAsync(RoleManager<RolesSistema> roleManager)
    {
      string[] roleNames = { "Administrador", "Usuario" };
      foreach (var roleName in roleNames)
      {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
          var role = new RolesSistema
          {
            Name = roleName,
            // NormalizedName se establece automáticamente
            DescripcionRol = roleName == "Administrador" ? "Acceso total y configuración del sistema." : "Acceso para gestión de datos operativos de la ONG."
          };
          await roleManager.CreateAsync(role);
          Console.WriteLine($"Rol '{roleName}' creado.");
        }
      }
    }

    private static async Task SeedAdminUserAsync(UserManager<UsuariosSistema> userManager, string email, string password, string nombres, string apellidos)
    {
      if (await userManager.FindByEmailAsync(email) == null)
      {
        var user = new UsuariosSistema
        {
          UserName = email, // Usar email como UserName por defecto
          Email = email,
          Nombres = nombres,
          Apellidos = apellidos,
          Activo = true,
          EmailConfirmed = true // Confirmar email para admin por defecto
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
          // Asignar rol de Administrador al usuario admin
          await userManager.AddToRoleAsync(user, "Administrador");
          Console.WriteLine($"Usuario administrador '{email}' creado y asignado al rol Administrador.");
        }
        else
        {
          Console.WriteLine($"Error al crear usuario administrador '{email}':");
          foreach (var error in result.Errors)
          {
            Console.WriteLine($"- {error.Description}");
          }
        }
      }
    }

    private static async Task SeedRolPermisosAsync(VNCenterDbContext context, RoleManager<RolesSistema> roleManager)
    {
      var adminRole = await roleManager.FindByNameAsync("Administrador");
      var userRole = await roleManager.FindByNameAsync("Usuario");
      var todosLosPermisos = await context.Permisos.ToListAsync();

      if (adminRole != null)
      {
        var permisosAdminActuales = await context.RolPermisos
                                            .Where(rp => rp.RolUsuarioID == adminRole.Id)
                                            .Select(rp => rp.PermisoID)
                                            .ToListAsync();

        foreach (var permiso in todosLosPermisos)
        {
          if (!permisosAdminActuales.Contains(permiso.PermisoID))
          {
            context.RolPermisos.Add(new RolPermisos { RolUsuarioID = adminRole.Id, PermisoID = permiso.PermisoID });
          }
        }
        Console.WriteLine($"Permisos para Administrador asignados/verificados.");
      }

      if (userRole != null)
      {
        var nombresPermisosUsuario = new List<string> {
                    "Dashboard_Ver", "Solicitud_Crear", "Solicitud_Listar", "Solicitud_VerDetalle",
                    "Solicitud_ActualizarEstado", "Beneficiario_Crear", "Beneficiario_Listar",
                    "Beneficiario_VerDetalle", "Beneficiario_Actualizar", "ProgramaProyecto_Listar",
                    "ProgramaProyecto_VerDetalle", "Comunidad_Gestionar", "GrupoComunitario_Gestionar",
                    "Participacion_Asignar", "Participacion_Listar", "Participacion_ActualizarDetalles",
                    "BeneficiarioPrograma_Asignar", "BeneficiarioPrograma_Listar",
                    "BeneficiarioPrograma_ActualizarEstado", "Evaluacion_Registrar", "Evaluacion_Listar",
                    "Evaluacion_VerDetalle", "InfoGeneral_Listar", "InfoGeneral_VerDetalle",
                    "InfoGeneral_ActualizarEstado", "Reporte_Generar"
                };

        var permisosParaUsuario = todosLosPermisos.Where(p => nombresPermisosUsuario.Contains(p.NombrePermiso)).ToList();
        var permisosUsuarioActuales = await context.RolPermisos
                                            .Where(rp => rp.RolUsuarioID == userRole.Id)
                                            .Select(rp => rp.PermisoID)
                                            .ToListAsync();

        foreach (var permiso in permisosParaUsuario)
        {
          if (!permisosUsuarioActuales.Contains(permiso.PermisoID))
          {
            context.RolPermisos.Add(new RolPermisos { RolUsuarioID = userRole.Id, PermisoID = permiso.PermisoID });
          }
        }
        Console.WriteLine($"Permisos para Usuario asignados/verificados.");
      }
      await context.SaveChangesAsync(); // Guardar cambios de RolPermisos
    }
  }
}
