// VN_Center/Documents/UsuariosSistemaPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.ViewModels; // Usando el ViewModel
using System.Collections.Generic;
using System.Linq;
using System;

namespace VN_Center.Documents
{
  public class UsuariosSistemaPdfDocument : IDocument
  {
    private readonly IEnumerable<UsuarioSistemaViewModel> _usuarios;
    private readonly string _logoPath;

    public UsuariosSistemaPdfDocument(IEnumerable<UsuarioSistemaViewModel> usuarios, string logoPath)
    {
      _usuarios = usuarios;
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
              column.Item().Text("Lista de Usuarios del Sistema")
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
        // Definición de las columnas de la tabla (ACTUALIZADO)
        table.ColumnsDefinition(columns =>
        {
          columns.RelativeColumn(3f);   // Nombre Completo
          columns.RelativeColumn(3f);   // Correo Electrónico
          columns.RelativeColumn(2.5f); // Roles
          columns.ConstantColumn(70f);  // Activo (tu propiedad personalizada)
        });

        // Encabezado de la tabla (ACTUALIZADO)
        table.Header(header =>
        {
          HeaderCellStyle(header.Cell()).Text("Nombre Completo");
          HeaderCellStyle(header.Cell()).Text("Correo Electrónico");
          HeaderCellStyle(header.Cell()).Text("Roles");
          HeaderCellStyle(header.Cell()).Text("Activo");
        });

        // Filas de datos para cada usuario (ACTUALIZADO)
        foreach (var usuario in _usuarios.OrderBy(u => u.NombreCompleto))
        {
          DataCellStyle(table.Cell()).Text(usuario.NombreCompleto ?? "N/A");
          DataCellStyle(table.Cell()).Text(usuario.Email ?? "N/A");
          DataCellStyle(table.Cell()).Text(usuario.Roles.Any() ? string.Join(", ", usuario.Roles) : "Sin roles");
          DataCellStyle(table.Cell()).AlignCenter().Text(usuario.Activo ? "Sí" : "No"); // Usando tu propiedad personalizada 'Activo'
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
