// VN_Center/Documents/EvaluacionesProgramaPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VN_Center.Documents
{
  public class EvaluacionesProgramaPdfDocument : IDocument
  {
    private readonly IEnumerable<EvaluacionesPrograma> _evaluaciones;
    private readonly string _logoPath;

    public EvaluacionesProgramaPdfDocument(IEnumerable<EvaluacionesPrograma> evaluaciones, string logoPath)
    {
      _evaluaciones = evaluaciones;
      _logoPath = logoPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(40);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeTable);
            page.Footer().Element(ComposeFooter);
          });
    }

    void ComposeHeader(IContainer container)
    {
      container
          .PaddingBottom(0.5f, Unit.Centimetre)
          .Row(row =>
          {
            if (System.IO.File.Exists(_logoPath))
            {
              row.RelativeItem(1).MaxHeight(60).Image(_logoPath);
            }
            else
            {
              row.RelativeItem(1).MaxHeight(60);
            }

            row.RelativeItem(4).PaddingLeft(10).Column(column =>
            {
              column.Item().Text("Lista de Evaluaciones de Programa")
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
        table.ColumnsDefinition(columns =>
        {
          columns.RelativeColumn(2.5f);
          columns.RelativeColumn(2.5f);
          columns.ConstantColumn(70f);
          columns.RelativeColumn(2f);
          columns.RelativeColumn(1.5f);
          columns.RelativeColumn(3f);
        });

        table.Header(header =>
        {
          HeaderCellStyle(header.Cell()).Text("Participante");
          HeaderCellStyle(header.Cell()).Text("Programa/Proyecto");
          HeaderCellStyle(header.Cell()).Text("Fecha Eval.");
          HeaderCellStyle(header.Cell()).Text("Evaluador");
          HeaderCellStyle(header.Cell()).Text("Recomienda");
          HeaderCellStyle(header.Cell()).Text("Comentarios Adic.");
        });

        foreach (var evaluacion in _evaluaciones.OrderByDescending(e => e.FechaEvaluacion))
        {
          string participanteNombre = "N/A";
          if (evaluacion.ParticipacionActiva?.Solicitud != null)
          {
            participanteNombre = $"{evaluacion.ParticipacionActiva.Solicitud.Nombres} {evaluacion.ParticipacionActiva.Solicitud.Apellidos}".Trim();
            if (string.IsNullOrWhiteSpace(participanteNombre)) participanteNombre = "Solicitante Desconocido";
          }

          string programaNombre = evaluacion.ParticipacionActiva?.ProgramaProyecto?.NombreProgramaProyecto ?? "N/A";

          DataCellStyle(table.Cell()).Text(participanteNombre);
          DataCellStyle(table.Cell()).Text(programaNombre);
          DataCellStyle(table.Cell()).Text(evaluacion.FechaEvaluacion.ToString("dd/MM/yy"));

          // ***** SECCIÓN MODIFICADA PARA LA CELDA DEL EVALUADOR *****
          table.Cell() // Obtener la celda
              .AlignCenter() // Indicar que el contenido de ESTA CELDA debe estar centrado. Devuelve IContainer.
              .BorderBottom(1).BorderColor(Colors.Grey.Lighten2) // Aplicar estilos a la celda. Devuelve IContainer.
              .PaddingVertical(4).PaddingHorizontal(5)           // Aplicar estilos a la celda. Devuelve IContainer.
              .DefaultTextStyle(x => x.FontSize(8))              // Aplicar estilos a la celda. Devuelve IContainer.
              .Text(evaluacion.NombreProgramaUniversidadEvaluador ?? ""); // Añadir el texto. Devuelve void.
                                                                          // ***** FIN DE SECCIÓN MODIFICADA *****

          DataCellStyle(table.Cell()).Text(evaluacion.RecomendariaProgramaOtros ?? "");
          DataCellStyle(table.Cell()).Text(evaluacion.ComentariosAdicionalesEvaluacion ?? "");
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
      return cellContainer.DefaultTextStyle(x => x.SemiBold().FontSize(9)).PaddingVertical(5).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    static IContainer DataCellStyle(IContainer cellContainer)
    {
      return cellContainer.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(5).DefaultTextStyle(x => x.FontSize(8));
    }
  }
}
