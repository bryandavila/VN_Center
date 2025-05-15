using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using VN_Center.Models.Entities;

namespace VN_Center.Documents//
{
  public class BeneficiariosPdfDocument : IDocument // IDocument es de QuestPDF
  {
    private readonly IEnumerable<Beneficiarios> _beneficiarios;
    private readonly string _reportTitle = "Lista de Beneficiarios";

    public BeneficiariosPdfDocument(IEnumerable<Beneficiarios> beneficiarios)
    {
      _beneficiarios = beneficiarios;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default; // DocumentMetadata es de QuestPDF

    public void Compose(IDocumentContainer container) // IDocumentContainer es de QuestPDF
    {
      container
          .Page(page =>
          {
            page.Margin(30, Unit.Point); // Unit es de QuestPDF
            page.DefaultTextStyle(style => style.FontSize(10).FontFamily(Fonts.Arial)); // Fonts es de QuestPDF

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(text =>
            {
              text.CurrentPageNumber();
              text.Span(" / ");
              text.TotalPages();
            });
          });
    }

    void ComposeHeader(IContainer container) // IContainer es de QuestPDF
    {
      container.Row(row =>
      {
        row.RelativeItem().Column(column =>
        {
          column.Item().Text(_reportTitle)
              .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium); // Colors es de QuestPDF

          column.Item().Text($"Generado el: {System.DateTime.Now:dd/MM/yyyy HH:mm}")
              .FontSize(9);
        });
      });
    }

    void ComposeContent(IContainer container) // IContainer es de QuestPDF
    {
      container.PaddingVertical(10, Unit.Point).Column(column =>
      {
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(1.5f); // Nombres
            columns.RelativeColumn(1.5f); // Apellidos
            columns.RelativeColumn(1.5f); // Comunidad
            columns.RelativeColumn(1);    // Rango Edad
            columns.RelativeColumn(1);    // Género
            columns.RelativeColumn(1.2f); // Fecha Registro
          });

          table.Header(header =>
          {
            static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);

            header.Cell().Element(CellStyle).Text("Nombres").SemiBold();
            header.Cell().Element(CellStyle).Text("Apellidos").SemiBold();
            header.Cell().Element(CellStyle).Text("Comunidad").SemiBold();
            header.Cell().Element(CellStyle).Text("Rango Edad").SemiBold();
            header.Cell().Element(CellStyle).Text("Género").SemiBold();
            header.Cell().Element(CellStyle).Text("Fecha Registro").SemiBold();
          });

          if (_beneficiarios != null && _beneficiarios.Any())
          {
            foreach (var beneficiario in _beneficiarios)
            {
              static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(3);

              table.Cell().Element(CellStyle).Text(beneficiario.Nombres ?? "N/A");
              table.Cell().Element(CellStyle).Text(beneficiario.Apellidos ?? "N/A");
              table.Cell().Element(CellStyle).Text(beneficiario.Comunidad?.NombreComunidad ?? "N/A");
              table.Cell().Element(CellStyle).Text(beneficiario.RangoEdad ?? "N/A");
              table.Cell().Element(CellStyle).Text(beneficiario.Genero ?? "N/A");
              table.Cell().Element(CellStyle).Text(beneficiario.FechaRegistroBeneficiario.ToString("dd/MM/yyyy"));
            }
          }
          else
          {
            table.Cell().ColumnSpan(6).PaddingTop(10).AlignCenter().Text("No hay beneficiarios para mostrar.").Italic();
          }
        });
      });
    }
  }
}
