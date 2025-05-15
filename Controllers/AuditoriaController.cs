// VN_Center/Controllers/AuditoriaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VN_Center.Data;
using VN_Center.Models.Entities;
using VN_Center.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace VN_Center.Controllers
{
  [Authorize(Roles = "Administrador")]
  public class AuditoriaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;

    public AuditoriaController(VNCenterDbContext context, UserManager<UsuariosSistema> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: Auditoria
    public async Task<IActionResult> Index(
        string? filtroUsuario,
        string? filtroTipoEvento,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pagina = 1)
    {
      ViewData["FiltroUsuarioActual"] = filtroUsuario;
      ViewData["FiltroTipoEventoActual"] = filtroTipoEvento;
      ViewData["FechaDesdeActual"] = fechaDesde?.ToString("yyyy-MM-dd");
      ViewData["FechaHastaActual"] = fechaHasta?.ToString("yyyy-MM-dd");

      var query = _context.RegistrosAuditoria.AsQueryable();

      // Filtro por Usuario Ejecutor (CORREGIDO y insensible a mayúsculas/minúsculas)
      if (!string.IsNullOrEmpty(filtroUsuario))
      {
        // Se busca en la columna NombreUsuarioEjecutor que ya guardamos.
        // ToLower() para búsqueda insensible a mayúsculas/minúsculas.
        query = query.Where(r => r.NombreUsuarioEjecutor != null && r.NombreUsuarioEjecutor.ToLower().Contains(filtroUsuario.ToLower()));
      }

      // Filtro por Tipo de Evento
      if (!string.IsNullOrEmpty(filtroTipoEvento))
      {
        query = query.Where(r => r.TipoEvento.Contains(filtroTipoEvento));
      }

      // Filtro por Fecha Desde
      if (fechaDesde.HasValue)
      {
        query = query.Where(r => r.FechaHoraEvento >= fechaDesde.Value);
      }

      // Filtro por Fecha Hasta
      if (fechaHasta.HasValue)
      {
        // Se añade un día a fechaHasta para incluir todos los eventos de ese día.
        query = query.Where(r => r.FechaHoraEvento < fechaHasta.Value.AddDays(1));
      }

      query = query.OrderByDescending(r => r.FechaHoraEvento);

      int pageSize = 20;
      var totalRegistros = await query.CountAsync();
      var registrosAuditoria = await query
                                      .Skip((pagina - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

      var viewModelList = new List<RegistroAuditoriaViewModel>();
      foreach (var registro in registrosAuditoria)
      {
        var vm = new RegistroAuditoriaViewModel
        {
          AuditoriaID = registro.AuditoriaID,
          FechaHoraEvento = registro.FechaHoraEvento,
          UsuarioEjecutorId = registro.UsuarioEjecutorId,
          NombreUsuarioEjecutor = registro.NombreUsuarioEjecutor,
          TipoEvento = registro.TipoEvento,
          EntidadAfectada = registro.EntidadAfectada,
          IdEntidadAfectada = registro.IdEntidadAfectada,
          DetallesCambio = registro.DetallesCambio,
          DireccionIp = registro.DireccionIp,
          NombreDetalleEntidadAfectada = registro.IdEntidadAfectada
        };

        if (registro.EntidadAfectada == "UsuariosSistema" && !string.IsNullOrEmpty(registro.IdEntidadAfectada))
        {
          // Intentamos encontrar el usuario por ID. El ID de UsuariosSistema es int.
          if (int.TryParse(registro.IdEntidadAfectada, out int userIdAfectado))
          {
            var usuarioAfectado = await _userManager.FindByIdAsync(userIdAfectado.ToString());
            if (usuarioAfectado != null)
            {
              vm.NombreDetalleEntidadAfectada = $"{usuarioAfectado.NombreCompleto} ({usuarioAfectado.UserName})";
            }
            else
            {
              vm.NombreDetalleEntidadAfectada = $"Usuario ID: {registro.IdEntidadAfectada} (No encontrado)";
            }
          }
          else
          {
            vm.NombreDetalleEntidadAfectada = $"ID Entidad: {registro.IdEntidadAfectada} (Formato incorrecto)";
          }
        }
        viewModelList.Add(vm);
      }

      ViewData["PaginaActual"] = pagina;
      ViewData["TotalPaginas"] = (int)Math.Ceiling(totalRegistros / (double)pageSize);
      ViewData["TienePaginaAnterior"] = pagina > 1;
      ViewData["TienePaginaSiguiente"] = pagina < (int)Math.Ceiling(totalRegistros / (double)pageSize);

      ViewBag.TiposEventoUnicos = await _context.RegistrosAuditoria
                                          .Select(r => r.TipoEvento)
                                          .Distinct()
                                          .OrderBy(t => t)
                                          .ToListAsync();

      return View(viewModelList);
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

      var viewModel = new RegistroAuditoriaViewModel
      {
        AuditoriaID = registroAuditoria.AuditoriaID,
        FechaHoraEvento = registroAuditoria.FechaHoraEvento,
        UsuarioEjecutorId = registroAuditoria.UsuarioEjecutorId,
        NombreUsuarioEjecutor = registroAuditoria.NombreUsuarioEjecutor,
        TipoEvento = registroAuditoria.TipoEvento,
        EntidadAfectada = registroAuditoria.EntidadAfectada,
        IdEntidadAfectada = registroAuditoria.IdEntidadAfectada,
        DetallesCambio = registroAuditoria.DetallesCambio,
        DireccionIp = registroAuditoria.DireccionIp,
        NombreDetalleEntidadAfectada = registroAuditoria.IdEntidadAfectada
      };

      if (registroAuditoria.EntidadAfectada == "UsuariosSistema" && !string.IsNullOrEmpty(registroAuditoria.IdEntidadAfectada))
      {
        if (int.TryParse(registroAuditoria.IdEntidadAfectada, out int userIdAfectado))
        {
          var usuarioAfectado = await _userManager.FindByIdAsync(userIdAfectado.ToString());
          if (usuarioAfectado != null)
          {
            viewModel.NombreDetalleEntidadAfectada = $"{usuarioAfectado.NombreCompleto} ({usuarioAfectado.UserName})";
          }
          else
          {
            viewModel.NombreDetalleEntidadAfectada = $"Usuario ID: {registroAuditoria.IdEntidadAfectada} (No encontrado)";
          }
        }
        else
        {
          viewModel.NombreDetalleEntidadAfectada = $"ID Entidad: {registroAuditoria.IdEntidadAfectada} (Formato incorrecto)";
        }
      }

      return View(viewModel);
    }
  }
}
