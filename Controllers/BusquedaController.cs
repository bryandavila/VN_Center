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

      if (!string.IsNullOrWhiteSpace(terminoBusqueda))
      {
        string busquedaLower = terminoBusqueda.ToLower().Trim();
        int limitePorCategoria = 10;

        // Buscar en Usuarios del Sistema
        resultados.UsuariosEncontrados = await _userManager.Users
            .Where(u => (u.UserName != null && u.UserName.ToLower().Contains(busquedaLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(busquedaLower)) ||
                        (u.Nombres != null && u.Nombres.ToLower().Contains(busquedaLower)) ||
                        (u.Apellidos != null && u.Apellidos.ToLower().Contains(busquedaLower)))
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Beneficiarios
        resultados.BeneficiariosEncontrados = await _context.Beneficiarios
            .Where(b => (b.Nombres != null && b.Nombres.ToLower().Contains(busquedaLower)) ||
                        (b.Apellidos != null && b.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (b.PaisOrigen != null && b.PaisOrigen.ToLower().Contains(busquedaLower)) ||
                        (b.EstadoMigratorio != null && b.EstadoMigratorio.ToLower().Contains(busquedaLower)) ||
                        // CORREGIDO: Usar un campo existente como TipoSituacionLaboral en lugar de ProfesionOcupacion
                        (b.TipoSituacionLaboral != null && b.TipoSituacionLaboral.ToLower().Contains(busquedaLower)) ||
                        (b.RangoEdad != null && b.RangoEdad.ToLower().Contains(busquedaLower)) ||
                        (b.Genero != null && b.Genero.ToLower().Contains(busquedaLower))
                        )
            .Include(b => b.Comunidad)
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Programas/Proyectos ONG
        resultados.ProgramasEncontrados = await _context.ProgramasProyectosONG
            .Where(p => (p.NombreProgramaProyecto != null && p.NombreProgramaProyecto.ToLower().Contains(busquedaLower)) ||
                        (p.Descripcion != null && p.Descripcion.ToLower().Contains(busquedaLower)) ||
                        (p.TipoIniciativa != null && p.TipoIniciativa.ToLower().Contains(busquedaLower)) ||
                        (p.EstadoProgramaProyecto != null && p.EstadoProgramaProyecto.ToLower().Contains(busquedaLower))
                        )
            .Include(p => p.ResponsablePrincipalONG)
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Solicitudes (Voluntariado/Pasantía)
        resultados.SolicitudesVolPasEncontradas = await _context.Solicitudes
            .Where(s => (s.Nombres != null && s.Nombres.ToLower().Contains(busquedaLower)) ||
                        (s.Apellidos != null && s.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (s.Email != null && s.Email.ToLower().Contains(busquedaLower)) ||
                        (s.TipoSolicitud != null && s.TipoSolicitud.ToLower().Contains(busquedaLower)) ||
                        (s.ProfesionOcupacion != null && s.ProfesionOcupacion.ToLower().Contains(busquedaLower)) || // ProfesionOcupacion SÍ existe en Solicitudes
                        (s.MotivacionInteresCR != null && s.MotivacionInteresCR.ToLower().Contains(busquedaLower)) ||
                        (s.EstadoSolicitud != null && s.EstadoSolicitud.ToLower().Contains(busquedaLower))
                        )
            .Include(s => s.NivelesIdioma)
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Solicitudes de Información General
        resultados.SolicitudesInfoEncontradas = await _context.SolicitudesInformacionGeneral
            .Where(s => (s.NombreContacto != null && s.NombreContacto.ToLower().Contains(busquedaLower)) ||
                        (s.EmailContacto != null && s.EmailContacto.ToLower().Contains(busquedaLower)) ||
                        (s.ProgramaDeInteres != null && s.ProgramaDeInteres.ToLower().Contains(busquedaLower)) ||
                        (s.PreguntasEspecificas != null && s.PreguntasEspecificas.ToLower().Contains(busquedaLower)) ||
                        (s.EstadoSolicitudInfo != null && s.EstadoSolicitudInfo.ToLower().Contains(busquedaLower))
                        )
            .Include(s => s.UsuarioAsignado)
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Comunidades
        resultados.ComunidadesEncontradas = await _context.Comunidades
            .Where(c => (c.NombreComunidad != null && c.NombreComunidad.ToLower().Contains(busquedaLower)) ||
                        (c.UbicacionDetallada != null && c.UbicacionDetallada.ToLower().Contains(busquedaLower)) ||
                        (c.NotasComunidad != null && c.NotasComunidad.ToLower().Contains(busquedaLower))
                        )
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Grupos Comunitarios
        resultados.GruposEncontrados = await _context.GruposComunitarios
            .Where(g => (g.NombreGrupo != null && g.NombreGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.DescripcionGrupo != null && g.DescripcionGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.TipoGrupo != null && g.TipoGrupo.ToLower().Contains(busquedaLower)) ||
                        (g.PersonaContactoPrincipal != null && g.PersonaContactoPrincipal.ToLower().Contains(busquedaLower))
                        )
            .Include(g => g.Comunidad)
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Evaluaciones de Programa
        resultados.EvaluacionesEncontradas = await _context.EvaluacionesPrograma
            .Include(e => e.ParticipacionActiva.Solicitud)
            .Include(e => e.ParticipacionActiva.ProgramaProyecto)
            .Where(e => (e.NombreProgramaUniversidadEvaluador != null && e.NombreProgramaUniversidadEvaluador.ToLower().Contains(busquedaLower)) ||
                        (e.ParteMasGratificante != null && e.ParteMasGratificante.ToLower().Contains(busquedaLower)) ||
                        (e.ParteMasDificil != null && e.ParteMasDificil.ToLower().Contains(busquedaLower)) ||
                        (e.ComentariosAdicionalesEvaluacion != null && e.ComentariosAdicionalesEvaluacion.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.Solicitud.Nombres != null && e.ParticipacionActiva.Solicitud.Nombres.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.Solicitud.Apellidos != null && e.ParticipacionActiva.Solicitud.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (e.ParticipacionActiva.ProgramaProyecto.NombreProgramaProyecto != null && e.ParticipacionActiva.ProgramaProyecto.NombreProgramaProyecto.ToLower().Contains(busquedaLower))
                        )
            .Take(limitePorCategoria)
            .ToListAsync();

        // Buscar en Participaciones Activas
        resultados.ParticipacionesEncontradas = await _context.ParticipacionesActivas
            .Include(p => p.Solicitud)
            .Include(p => p.ProgramaProyecto)
            .Where(p => (p.RolDesempenado != null && p.RolDesempenado.ToLower().Contains(busquedaLower)) ||
                        (p.NotasSupervisor != null && p.NotasSupervisor.ToLower().Contains(busquedaLower)) ||
                        (p.Solicitud.Nombres != null && p.Solicitud.Nombres.ToLower().Contains(busquedaLower)) ||
                        (p.Solicitud.Apellidos != null && p.Solicitud.Apellidos.ToLower().Contains(busquedaLower)) ||
                        (p.ProgramaProyecto.NombreProgramaProyecto != null && p.ProgramaProyecto.NombreProgramaProyecto.ToLower().Contains(busquedaLower))
                        )
            .Take(limitePorCategoria)
            .ToListAsync();
      }
      return View(resultados);
    }
  }
}
