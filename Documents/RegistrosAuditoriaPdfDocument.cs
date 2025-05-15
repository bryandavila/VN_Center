// VN_Center/Documents/RegistrosAuditoriaPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.ViewModels; // Para RegistroAuditoriaViewModel
using System.Collections.Generic;
using System.Linq;
using System;

namespace VN_Center.Documents
{
  public class RegistrosAuditoriaPdfDocument : IDocument
  {
    private readonly IEnumerable<RegistroAuditoriaViewModel> _registros;
    private readonly string _logoPath;
    private readonly string _filtrosAplicados; // Para mostrar los filtros en el PDF

    public RegistrosAuditoriaPdfDocument(IEnumerable<RegistroAuditoriaViewModel> registros, string logoPath, string filtrosAplicados)
    {
      _registros = registros;
      _logoPath = logoPath;
      _filtrosAplicados = filtrosAplicados;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(30); // Margen un poco más reducido para más contenido

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeTable);
            page.Footer().Element(ComposeFooter);
          });
    }

    void ComposeHeader(IContainer container)
    {
      container
          .PaddingBottom(0.75f, Unit.Centimetre)
          .Column(col => {
            col.Item().Row(row =>
            {
              if (System.IO.File.Exists(_logoPath))
              {
                row.RelativeItem(1).MaxHeight(60).Image(_logoPath);
              }
              else
              {
                row.RelativeItem(1).MaxHeight(60); // Espacio para el logo
              }

              row.RelativeItem(4).PaddingLeft(10).Column(column =>
              {
                column.Item().Text("Registros de Auditoría del Sistema")
                          .Bold().FontSize(18).FontColor(Colors.Blue.Medium);
                column.Item().Text("VN Center")
                          .SemiBold().FontSize(12);
              });
            });

            if (!string.IsNullOrWhiteSpace(_filtrosAplicados))
            {
              col.Item().PaddingTop(5).Text(text =>
              {
                text.Span("Filtros Aplicados: ").SemiBold().FontSize(9);
                text.Span(_filtrosAplicados).FontSize(9);
              });
            }
          });
    }

    void ComposeTable(IContainer container)
    {
      container.Table(table =>
      {
        table.ColumnsDefinition(columns =>
        {
          columns.ConstantColumn(100f); // Fecha y Hora
          columns.RelativeColumn(2f);  // Usuario Ejecutor
          columns.RelativeColumn(1.5f); // Tipo de Evento
          columns.RelativeColumn(2f);   // Entidad Afectada (Nombre/Detalle)
          columns.RelativeColumn(3f);   // Detalles del Cambio (puede ser largo)
          columns.RelativeColumn(1.5f); // Dirección IP
        });

        table.Header(header =>
        {
          HeaderCellStyle(header.Cell()).Text("Fecha y Hora");
          HeaderCellStyle(header.Cell()).Text("Usuario Ejecutor");
          HeaderCellStyle(header.Cell()).Text("Tipo Evento");
          HeaderCellStyle(header.Cell()).Text("Entidad/Usuario Afectado");
          HeaderCellStyle(header.Cell()).Text("Detalles del Cambio");
          HeaderCellStyle(header.Cell()).Text("Dirección IP");
        });

        foreach (var registro in _registros) // Ya deberían estar ordenados por el controlador
        {
          DataCellStyle(table.Cell()).Text(registro.FechaHoraEvento.ToString("dd/MM/yy HH:mm:ss"));
          DataCellStyle(table.Cell()).Text(registro.NombreUsuarioEjecutor ?? "N/A");
          DataCellStyle(table.Cell()).Text(registro.TipoEvento);
          DataCellStyle(table.Cell()).Text(registro.NombreDetalleEntidadAfectada ?? registro.EntidadAfectada ?? "N/A");
          DataCellStyle(table.Cell()).Text(registro.DetallesCambio ?? "");
          DataCellStyle(table.Cell()).Text(registro.DireccionIp ?? "N/A");
        }
      });
    }

    void ComposeFooter(IContainer container)
    {
      container
          .AlignCenter()
          .DefaultTextStyle(style => style.FontSize(9f))
          .Text(text =>
          {
            text.Span("Página ");
            text.CurrentPageNumber();
            text.Span(" de ");
            text.TotalPages();
            text.Span(" | Generado el: ");
            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
          });
    }

    static IContainer HeaderCellStyle(IContainer cellContainer)
    {
      return cellContainer.DefaultTextStyle(x => x.SemiBold().FontSize(8)).PaddingVertical(4).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingHorizontal(4);
    }

    static IContainer DataCellStyle(IContainer cellContainer)
    {
      return cellContainer.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).PaddingHorizontal(4).DefaultTextStyle(x => x.FontSize(7));
    }
  }
}
