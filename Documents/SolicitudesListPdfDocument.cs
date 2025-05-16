// VN_Center/Documents/SolicitudesListPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities; // Para la entidad Solicitudes
using System.Collections.Generic;
using System.Linq;
using System;

namespace VN_Center.Documents
{
  public class SolicitudesListPdfDocument : IDocument
  {
    private readonly IEnumerable<Solicitudes> _solicitudes;
    private readonly string _logoPath;
    private readonly string _tituloReporte;

    public SolicitudesListPdfDocument(IEnumerable<Solicitudes> solicitudes, string logoPath, string tituloReporte)
    {
      _solicitudes = solicitudes;
      _logoPath = logoPath;
      _tituloReporte = tituloReporte;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(30);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeTable);
            page.Footer().Element(ComposeFooter);
          });
    }

    void ComposeHeader(IContainer container)
    {
      container
          .PaddingBottom(0.75f, Unit.Centimetre)
          .Row(row =>
          {
            if (System.IO.File.Exists(_logoPath))
            {
              row.RelativeItem(1).MaxHeight(60).Image(_logoPath);
            }
            else
            {
              row.RelativeItem(1).MaxHeight(60); // Espacio reservado
            }

            row.RelativeItem(4).PaddingLeft(10).Column(column =>
            {
              column.Item().Text(_tituloReporte)
                        .Bold().FontSize(18).FontColor(Colors.Blue.Medium);
              column.Item().Text("VN Center")
                        .SemiBold().FontSize(12);
            });
          });
    }

    void ComposeTable(IContainer container)
    {
      container.Table(table =>
      {
        // Definición de columnas para la lista, basadas en tu Index.cshtml
        table.ColumnsDefinition(columns =>
        {
          columns.RelativeColumn(2f);   // Nombres
          columns.RelativeColumn(2f);   // Apellidos
          columns.RelativeColumn(3f);   // Email
          columns.RelativeColumn(1.5f); // Tipo Solicitud
          columns.RelativeColumn(1.5f); // Estado
          columns.ConstantColumn(75f);  // Fecha Envío
        });

        // Encabezado de la tabla
        table.Header(header =>
        {
          HeaderCellStyle(header.Cell()).Text("Nombres");
          HeaderCellStyle(header.Cell()).Text("Apellidos");
          HeaderCellStyle(header.Cell()).Text("Email");
          HeaderCellStyle(header.Cell()).Text("Tipo");
          HeaderCellStyle(header.Cell()).Text("Estado");
          HeaderCellStyle(header.Cell()).Text("Fecha Envío");
        });

        // Filas de datos
        foreach (var solicitud in _solicitudes) // Asumimos que ya vienen ordenadas del controlador
        {
          DataCellStyle(table.Cell()).Text(solicitud.Nombres);
          DataCellStyle(table.Cell()).Text(solicitud.Apellidos);
          DataCellStyle(table.Cell()).Text(solicitud.Email);
          DataCellStyle(table.Cell()).Text(solicitud.TipoSolicitud);
          DataCellStyle(table.Cell()).Text(solicitud.EstadoSolicitud);
          DataCellStyle(table.Cell()).Text(solicitud.FechaEnvioSolicitud.ToString("dd/MM/yyyy"));
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
      return cellContainer.DefaultTextStyle(x => x.SemiBold().FontSize(9)).PaddingVertical(5).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingHorizontal(4);
    }

    static IContainer DataCellStyle(IContainer cellContainer)
    {
      return cellContainer.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4).DefaultTextStyle(x => x.FontSize(8));
    }
  }
}
