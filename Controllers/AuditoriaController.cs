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
using QuestPDF.Fluent; // Para QuestPDF
using VN_Center.Documents; // Para RegistrosAuditoriaPdfDocument
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment
using System.IO; // Para Path
using System.Text; // Para StringBuilder

namespace VN_Center.Controllers
{
  [Authorize(Roles = "Administrador")]
  public class AuditoriaController : Controller
  {
    private readonly VNCenterDbContext _context;
    private readonly UserManager<UsuariosSistema> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment; // <--- AÑADIDO

    public AuditoriaController(
        VNCenterDbContext context,
        UserManager<UsuariosSistema> userManager,
        IWebHostEnvironment webHostEnvironment) // <--- AÑADIDO
    {
      _context = context;
      _userManager = userManager;
      _webHostEnvironment = webHostEnvironment; // <--- AÑADIDO
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

      if (!string.IsNullOrEmpty(filtroUsuario))
      {
        query = query.Where(r => r.NombreUsuarioEjecutor != null && r.NombreUsuarioEjecutor.ToLower().Contains(filtroUsuario.ToLower()));
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
      // ... (código existente de Details) ...
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

    // NUEVA ACCIÓN PARA EXPORTAR A PDF LA LISTA DE AUDITORÍA
    public async Task<IActionResult> ExportToPdf(
        string? filtroUsuario,
        string? filtroTipoEvento,
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
      var query = _context.RegistrosAuditoria.AsQueryable();
      var sbFiltros = new StringBuilder();

      if (!string.IsNullOrEmpty(filtroUsuario))
      {
        query = query.Where(r => r.NombreUsuarioEjecutor != null && r.NombreUsuarioEjecutor.ToLower().Contains(filtroUsuario.ToLower()));
        sbFiltros.Append($"Usuario Ejecutor: '{filtroUsuario}'; ");
      }

      if (!string.IsNullOrEmpty(filtroTipoEvento))
      {
        query = query.Where(r => r.TipoEvento.Contains(filtroTipoEvento));
        sbFiltros.Append($"Tipo Evento: '{filtroTipoEvento}'; ");
      }

      if (fechaDesde.HasValue)
      {
        query = query.Where(r => r.FechaHoraEvento >= fechaDesde.Value);
        sbFiltros.Append($"Desde: {fechaDesde.Value:dd/MM/yyyy}; ");
      }

      if (fechaHasta.HasValue)
      {
        query = query.Where(r => r.FechaHoraEvento < fechaHasta.Value.AddDays(1));
        sbFiltros.Append($"Hasta: {fechaHasta.Value:dd/MM/yyyy}; ");
      }

      // Obtener todos los registros que coinciden con los filtros (sin paginación para el PDF)
      var registrosAuditoria = await query.OrderByDescending(r => r.FechaHoraEvento).ToListAsync();

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

      string wwwRootPath = _webHostEnvironment.WebRootPath;
      string logoPath = Path.Combine(wwwRootPath, "img", "logo_vncenter_mini.png");
      string filtrosAplicadosStr = sbFiltros.Length > 0 ? sbFiltros.ToString().TrimEnd(';', ' ') : "Ninguno";

      var document = new RegistrosAuditoriaPdfDocument(viewModelList, logoPath, filtrosAplicadosStr);
      var pdfBytes = document.GeneratePdf();

      return File(pdfBytes, "application/pdf", $"RegistrosAuditoria_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }
  }
}
