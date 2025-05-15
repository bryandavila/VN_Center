// VN_Center/Documents/BeneficiariosPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VN_Center.Documents
{
  public class BeneficiariosPdfDocument : IDocument
  {
    private readonly IEnumerable<Beneficiarios> _beneficiarios;

    public BeneficiariosPdfDocument(IEnumerable<Beneficiarios> beneficiarios)
    {
      _beneficiarios = beneficiarios;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(50);

            page.Header()
                      .AlignCenter()
                      .Text("Lista de Beneficiarios - VN Center")
                      .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

            page.Content().Element(ComposeTable);

            page.Footer()
                      .AlignCenter()
                      .Text(text =>
                  {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                    text.Span(" | Generado el: ");
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                  });
          });
    }

    void ComposeTable(IContainer container)
    {
      container.Table(table =>
      {
        table.ColumnsDefinition(columns =>
        {
          columns.ConstantColumn(40f); // ID - Añadido sufijo 'f'
          columns.RelativeColumn(2f);  // Nombres - Añadido sufijo 'f'
          columns.RelativeColumn(2f);  // Apellidos - Añadido sufijo 'f'
          columns.RelativeColumn(1.5f); // Rango Edad - Añadido sufijo 'f'
          columns.RelativeColumn(1.5f); // Género - Añadido sufijo 'f'
          columns.RelativeColumn(2f);  // Comunidad - Añadido sufijo 'f'
          columns.RelativeColumn(2f);  // Fecha Registro - Añadido sufijo 'f'
        });

        table.Header(header =>
        {
          header.Cell().Element(CellStyle).Text("ID");
          header.Cell().Element(CellStyle).Text("Nombres");
          header.Cell().Element(CellStyle).Text("Apellidos");
          header.Cell().Element(CellStyle).Text("Rango Edad");
          header.Cell().Element(CellStyle).Text("Género");
          header.Cell().Element(CellStyle).Text("Comunidad");
          header.Cell().Element(CellStyle).Text("Fecha Registro");

          static IContainer CellStyle(IContainer cellContainer)
          {
            return cellContainer.DefaultTextStyle(x => x.SemiBold().FontSize(10)).PaddingVertical(5).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
          }
        });

        foreach (var beneficiario in _beneficiarios)
        {
          table.Cell().Element(DataCellStyle).Text(beneficiario.BeneficiarioID.ToString());
          table.Cell().Element(DataCellStyle).Text(beneficiario.Nombres ?? "N/A");
          table.Cell().Element(DataCellStyle).Text(beneficiario.Apellidos ?? "N/A");
          table.Cell().Element(DataCellStyle).Text(beneficiario.RangoEdad ?? "N/A");
          table.Cell().Element(DataCellStyle).Text(beneficiario.Genero ?? "N/A");
          table.Cell().Element(DataCellStyle).Text(beneficiario.Comunidad?.NombreComunidad ?? "N/A");
          table.Cell().Element(DataCellStyle).Text(beneficiario.FechaRegistroBeneficiario.ToString("dd/MM/yyyy"));

          static IContainer DataCellStyle(IContainer cellContainer) // Renombrado para evitar conflicto con el de header si estuvieran en el mismo scope exacto, aunque aquí es local al loop.
          {
            return cellContainer.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).DefaultTextStyle(x => x.FontSize(9));
          }
        }
      });
    }
  }
}
