// VN_Center/Controllers/BusquedaController.cs
using Microsoft.AspNetCore.Mvc;
using VN_Center.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VN_Center.Models.ViewModels;
using VN_Center.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VN_Center.Controllers
{
  [Authorize]
  public class BusquedaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public BusquedaController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: Busqueda/Resultados
    public async Task<IActionResult> Resultados(string terminoBusqueda)
    {
      ViewData["TerminoBusquedaOriginal"] = terminoBusqueda;
      var resultados = new ResultadosBusquedaViewModel
      {
        TerminoBusqueda = terminoBusqueda
      };

      var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      bool esAdmin = User.IsInRole("Administrador");

      if (!string.IsNullOrWhiteSpace(terminoBusqueda))
      {
        string busquedaLower = terminoBusqueda.ToLower().Trim();
        int limitePorCategoria = 10;

        // --- Usuarios del Sistema: Solo si es Admin ---
        if (esAdmin)
        {
          resultados.UsuariosEncontrados = await _userManager.Users
              .Where(u => (u.UserName != null && u.UserName.ToLower().Contains(busquedaLower)) ||
                          (u.Email != null && u.Email.ToLower().Contains(busquedaLower)) ||
                          (u.Nombres != null && u.Nombres.ToLower().Contains(busquedaLower)) ||
                          (u.Apellidos != null && u.Apellidos.ToLower().Contains(busquedaLower)))
              .Take(limitePorCategoria)
              .ToListAsync();
        }

        // --- Beneficiarios: Filtrado si no es Admin ---
        IQueryable<Beneficiarios> queryBeneficiarios = _context.Beneficiarios;
        if (!esAdmin && !string.IsNullOrEmpty(currentUserId))
        {
          queryBeneficiarios = queryBeneficiarios.Where(b => b.UsuarioCreadorId == currentUserId);
        }
        resultados.BeneficiariosEncontrados = await queryBeneficiarios
            .Where(b => (b.Nombres != null && b.Nombres.ToLower().Contains(busquedaLower)) ||
                        (b.Apellidos != null && b.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (b.PaisOrigen != null && b.PaisOrigen.ToLower().Contains(busquedaLower)) ||
                        (b.EstadoMigratorio != null && b.EstadoMigratorio.ToLower().Contains(busquedaLower)) ||
                        (b.TipoSituacionLaboral != null && b.TipoSituacionLaboral.ToLower().Contains(busquedaLower)) ||
                        (b.RangoEdad != null && b.RangoEdad.ToLower().Contains(busquedaLower)) ||
                        (b.Genero != null && b.Genero.ToLower().Contains(busquedaLower)))
            .Include(b => b.Comunidad)
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Programas/Proyectos ONG: Visible para todos ---
        resultados.ProgramasEncontrados = await _context.ProgramasProyectosONG
            .Where(p => (p.NombreProgramaProyecto != null && p.NombreProgramaProyecto.ToLower().Contains(busquedaLower)) ||
                        (p.Descripcion != null && p.Descripcion.ToLower().Contains(busquedaLower)) ||
                        (p.TipoIniciativa != null && p.TipoIniciativa.ToLower().Contains(busquedaLower)) ||
                        (p.EstadoProgramaProyecto != null && p.EstadoProgramaProyecto.ToLower().Contains(busquedaLower)))
            .Include(p => p.ResponsablePrincipalONG)
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Solicitudes (Voluntariado/Pasantía): Filtrado si no es Admin ---
        IQueryable<Solicitudes> querySolicitudesVolPas = _context.Solicitudes;
        if (!esAdmin && !string.IsNullOrEmpty(currentUserId))
        {
          querySolicitudesVolPas = querySolicitudesVolPas.Where(s => s.UsuarioCreadorId == currentUserId);
        }
        resultados.SolicitudesVolPasEncontradas = await querySolicitudesVolPas
            .Where(s => (s.Nombres != null && s.Nombres.ToLower().Contains(busquedaLower)) ||
                        (s.Apellidos != null && s.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (s.Email != null && s.Email.ToLower().Contains(busquedaLower)) ||
                        (s.TipoSolicitud != null && s.TipoSolicitud.ToLower().Contains(busquedaLower)) ||
                        (s.ProfesionOcupacion != null && s.ProfesionOcupacion.ToLower().Contains(busquedaLower)) ||
                        (s.MotivacionInteresCR != null && s.MotivacionInteresCR.ToLower().Contains(busquedaLower)) ||
                        (s.EstadoSolicitud != null && s.EstadoSolicitud.ToLower().Contains(busquedaLower)))
            .Include(s => s.NivelesIdioma)
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Solicitudes de Información General: Filtrado si no es Admin ---
        IQueryable<SolicitudesInformacionGeneral> querySolicitudesInfo = _context.SolicitudesInformacionGeneral;
        if (!esAdmin && !string.IsNullOrEmpty(currentUserId))
        {
          querySolicitudesInfo = querySolicitudesInfo.Where(s => s.UsuarioCreadorId == currentUserId);
        }
        resultados.SolicitudesInfoEncontradas = await querySolicitudesInfo
            .Where(s => (s.NombreContacto != null && s.NombreContacto.ToLower().Contains(busquedaLower)) ||
                        (s.EmailContacto != null && s.EmailContacto.ToLower().Contains(busquedaLower)) ||
                        (s.ProgramaDeInteres != null && s.ProgramaDeInteres.ToLower().Contains(busquedaLower)) ||
                        (s.PreguntasEspecificas != null && s.PreguntasEspecificas.ToLower().Contains(busquedaLower)) ||
                        (s.EstadoSolicitudInfo != null && s.EstadoSolicitudInfo.ToLower().Contains(busquedaLower)))
            .Include(s => s.UsuarioAsignado)
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Comunidades: Visible para todos ---
        resultados.ComunidadesEncontradas = await _context.Comunidades
            .Where(c => (c.NombreComunidad != null && c.NombreComunidad.ToLower().Contains(busquedaLower)) ||
                        (c.UbicacionDetallada != null && c.UbicacionDetallada.ToLower().Contains(busquedaLower)) ||
                        (c.NotasComunidad != null && c.NotasComunidad.ToLower().Contains(busquedaLower)))
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Grupos Comunitarios: Visible para todos ---
        resultados.GruposEncontrados = await _context.GruposComunitarios
            .Where(g => (g.NombreGrupo != null && g.NombreGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.DescripcionGrupo != null && g.DescripcionGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.TipoGrupo != null && g.TipoGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.PersonaContactoPrincipal != null && g.PersonaContactoPrincipal.ToLower().Contains(busquedaLower)))
            .Include(g => g.Comunidad)
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Evaluaciones de Programa: Filtrado si no es Admin ---
        IQueryable<EvaluacionesPrograma> queryEvaluaciones = _context.EvaluacionesPrograma
            .Include(e => e.ParticipacionActiva.Solicitud)
            .Include(e => e.ParticipacionActiva.ProgramaProyecto);
        if (!esAdmin && !string.IsNullOrEmpty(currentUserId))
        {
          // Un usuario no-admin solo ve evaluaciones de participaciones ligadas a solicitudes que él creó
          queryEvaluaciones = queryEvaluaciones.Where(e => e.ParticipacionActiva.Solicitud.UsuarioCreadorId == currentUserId);
        }
        resultados.EvaluacionesEncontradas = await queryEvaluaciones
            .Where(e => (e.NombreProgramaUniversidadEvaluador != null && e.NombreProgramaUniversidadEvaluador.ToLower().Contains(busquedaLower)) ||
                        (e.ParteMasGratificante != null && e.ParteMasGratificante.ToLower().Contains(busquedaLower)) ||
                        (e.ParteMasDificil != null && e.ParteMasDificil.ToLower().Contains(busquedaLower)) ||
                        (e.ComentariosAdicionalesEvaluacion != null && e.ComentariosAdicionalesEvaluacion.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.Solicitud.Nombres != null && e.ParticipacionActiva.Solicitud.Nombres.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.Solicitud.Apellidos != null && e.ParticipacionActiva.Solicitud.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.ProgramaProyecto.NombreProgramaProyecto != null && e.ParticipacionActiva.ProgramaProyecto.NombreProgramaProyecto.ToLower().Contains(busquedaLower)))
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Participaciones Activas: Filtrado si no es Admin ---
        IQueryable<ParticipacionesActivas> queryParticipaciones = _context.ParticipacionesActivas
            .Include(p => p.Solicitud)
            .Include(p => p.ProgramaProyecto);
        if (!esAdmin && !string.IsNullOrEmpty(currentUserId))
        {
          // Un usuario no-admin solo ve participaciones ligadas a solicitudes que él creó
          queryParticipaciones = queryParticipaciones.Where(p => p.Solicitud.UsuarioCreadorId == currentUserId);
        }
        resultados.ParticipacionesEncontradas = await queryParticipaciones
            .Where(p => (p.RolDesempenado != null && p.RolDesempenado.ToLower().Contains(busquedaLower)) ||
                        (p.NotasSupervisor != null && p.NotasSupervisor.ToLower().Contains(busquedaLower)) ||
                        (p.Solicitud.Nombres != null && p.Solicitud.Nombres.ToLower().Contains(busquedaLower)) ||
                        (p.Solicitud.Apellidos != null && p.Solicitud.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (p.ProgramaProyecto.NombreProgramaProyecto != null && p.ProgramaProyecto.NombreProgramaProyecto.ToLower().Contains(busquedaLower)))
            .Take(limitePorCategoria)
            .ToListAsync();

        // --- Módulos solo para Administradores ---
        if (esAdmin)
        {
          // Buscar en Registros de Auditoría
          resultados.AuditoriaEncontrada = await _context.RegistrosAuditoria
              .Where(a => (a.NombreUsuarioEjecutor != null && a.NombreUsuarioEjecutor.ToLower().Contains(busquedaLower)) ||
                          (a.TipoEvento != null && a.TipoEvento.ToLower().Contains(busquedaLower)) ||
                          (a.EntidadAfectada != null && a.EntidadAfectada.ToLower().Contains(busquedaLower)) ||
                          (a.IdEntidadAfectada != null && a.IdEntidadAfectada.ToLower().Contains(busquedaLower)) ||
                          (a.DetallesCambio != null && a.DetallesCambio.ToLower().Contains(busquedaLower)) ||
                          (a.DireccionIp != null && a.DireccionIp.Contains(busquedaLower)) // IP no suele ser case-sensitive
                          )
              .OrderByDescending(a => a.FechaHoraEvento) // Mostrar los más recientes primero
              .Take(limitePorCategoria)
              .ToListAsync();

          // Buscar en Catálogo: Niveles de Idioma
          resultados.NivelesIdiomaEncontrados = await _context.NivelesIdioma
              .Where(ni => ni.NombreNivel != null && ni.NombreNivel.ToLower().Contains(busquedaLower))
              .Take(limitePorCategoria)
              .ToListAsync();

          // Buscar en Vinculación: Beneficiarios en Programas/Proyectos
          resultados.BeneficiariosEnProgramasEncontrados = await _context.BeneficiariosProgramasProyectos
              .Include(bpp => bpp.Beneficiario)
              .Include(bpp => bpp.ProgramaProyecto)
              .Where(bpp => (bpp.Beneficiario.Nombres != null && bpp.Beneficiario.Nombres.ToLower().Contains(busquedaLower)) ||
                            (bpp.Beneficiario.Apellidos != null && bpp.Beneficiario.Apellidos.ToLower().Contains(busquedaLower)) ||
                            (bpp.ProgramaProyecto.NombreProgramaProyecto != null && bpp.ProgramaProyecto.NombreProgramaProyecto.ToLower().Contains(busquedaLower)) ||
                            (bpp.EstadoParticipacionBeneficiario != null && bpp.EstadoParticipacionBeneficiario.ToLower().Contains(busquedaLower)) ||
                            (bpp.NotasAdicionales != null && bpp.NotasAdicionales.ToLower().Contains(busquedaLower))
                            )
              .Take(limitePorCategoria)
              .ToListAsync();

          // Puedes añadir aquí la lógica para buscar en otras entidades de administración:
          // - BeneficiarioGrupos
          // - BeneficiarioAsistenciaRecibida
          // - ProgramaProyectoComunidades
          // - ProgramaProyectoGrupos
          // - SolicitudCamposInteres
          // - Otros Catálogos (FuentesConocimiento, TiposAsistencia, CamposInteresVocacional)
        }
      }
      return View(resultados);
    }
  }
}
