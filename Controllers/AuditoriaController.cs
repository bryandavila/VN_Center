// VN_Center/Controllers/AuditoriaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data; // Para VNCenterDbContext
using VN_Center.Models.Entities; // Para RegistrosAuditoria
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
// Para paginación, podríamos usar una librería o hacerlo manualmente.
// Por ahora, lo haremos simple y luego podemos añadir paginación.

namespace VN_Center.Controllers
{
  [Authorize(Roles = "Administrador")] // Solo los administradores pueden ver la auditoría
  public class AuditoriaController : Controller
  {
    private readonly VNCenterDbContext _context;

    public AuditoriaController(VNCenterDbContext context)
    {
      _context = context;
    }

    // GET: Auditoria
    public async Task<IActionResult> Index(
        string filtroUsuario,
        string filtroTipoEvento,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pagina = 1)
    {
      ViewData["FiltroUsuarioActual"] = filtroUsuario;
      ViewData["FiltroTipoEventoActual"] = filtroTipoEvento;
      ViewData["FechaDesdeActual"] = fechaDesde?.ToString("yyyy-MM-dd");
      ViewData["FechaHastaActual"] = fechaHasta?.ToString("yyyy-MM-dd");

      var query = _context.RegistrosAuditoria.AsQueryable();

      if (!string.IsNullOrEmpty(filtroUsuario))
      {
        query = query.Where(r => r.NombreUsuarioEjecutor != null && r.NombreUsuarioEjecutor.Contains(filtroUsuario));
      }

      if (!string.IsNullOrEmpty(filtroTipoEvento))
      {
        query = query.Where(r => r.TipoEvento.Contains(filtroTipoEvento));
      }

      if (fechaDesde.HasValue)
      {
        query = query.Where(r => r.FechaHoraEvento >= fechaDesde.Value);
      }

      if (fechaHasta.HasValue)
      {
        // Añadir un día a fechaHasta para incluir eventos de todo ese día
        query = query.Where(r => r.FechaHoraEvento < fechaHasta.Value.AddDays(1));
      }

      // Ordenar por fecha descendente para ver los más recientes primero
      query = query.OrderByDescending(r => r.FechaHoraEvento);

      // Paginación simple
      int pageSize = 20; // Elementos por página
      var totalRegistros = await query.CountAsync();
      var registrosPaginados = await query
                                      .Skip((pagina - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

      ViewData["PaginaActual"] = pagina;
      ViewData["TotalPaginas"] = (int)Math.Ceiling(totalRegistros / (double)pageSize);
      ViewData["TienePaginaAnterior"] = pagina > 1;
      ViewData["TienePaginaSiguiente"] = pagina < (int)Math.Ceiling(totalRegistros / (double)pageSize);

      // Obtener tipos de evento distintos para el dropdown de filtro
      ViewBag.TiposEventoUnicos = await _context.RegistrosAuditoria
                                          .Select(r => r.TipoEvento)
                                          .Distinct()
                                          .OrderBy(t => t)
                                          .ToListAsync();

      return View(registrosPaginados);
    }

    // GET: Auditoria/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var registroAuditoria = await _context.RegistrosAuditoria
          .FirstOrDefaultAsync(m => m.AuditoriaID == id);

      if (registroAuditoria == null)
      {
        return NotFound();
      }

      return View(registroAuditoria);
    }
  }
}
