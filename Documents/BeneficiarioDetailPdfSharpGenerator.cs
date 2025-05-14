using PdfSharpCore.Drawing;        // Para XGraphics, XFont, XBrush, XRect, XStringFormats, XFontStyle
using PdfSharpCore.Drawing.Layout; // Para XTextFormatter
using PdfSharpCore.Pdf;            // Para PdfDocument, PdfPage
using System.IO;
using VN_Center.Models.Entities;   // Asegúrate que este es el namespace correcto
using System;                     // Para System.DateTime

namespace VN_Center.Documents
{
  public class BeneficiarioDetailPdfSharpGenerator
  {
    private readonly Beneficiarios _beneficiario;

    public BeneficiarioDetailPdfSharpGenerator(Beneficiarios beneficiario)
    {
      _beneficiario = beneficiario;
    }

    public byte[] GeneratePdf()
    {
      PdfDocument document = new PdfDocument();
      document.Info.Title = $"Detalle Beneficiario - {_beneficiario.Nombres} {_beneficiario.Apellidos}";
      document.Info.Author = "VN Center";

      PdfPage page = document.AddPage();
      page.Size = PdfSharpCore.PageSize.Letter;

      using (XGraphics gfx = XGraphics.FromPdfPage(page))
      {
        // Definir fuentes usando XFontStyle
        XFont fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
        XFont fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
        XFont fontBody = new XFont("Arial", 10, XFontStyle.Regular);
        XFont fontSmall = new XFont("Arial", 8, XFontStyle.Italic);

        double yPosition = 40;
        double leftMargin = 40;
        double contentWidth = page.Width.Point - 2 * leftMargin;
        double defaultLineHeight = fontBody.GetHeight(); // Correcto: sin argumentos
        double sectionSpacing = 20;

        // Título del Documento
        gfx.DrawString("Detalle del Beneficiario", fontTitle, XBrushes.Black,
            new XRect(0, yPosition, page.Width.Point, defaultLineHeight * 2),
            XStringFormats.TopCenter);
        yPosition += defaultLineHeight * 2 + 10;

        gfx.DrawString($"ID Beneficiario: {_beneficiario.BeneficiarioID}", fontBody, XBrushes.Black, leftMargin, yPosition, XStringFormats.TopLeft);
        yPosition += defaultLineHeight;
        gfx.DrawString($"Generado el: {System.DateTime.Now:dd/MM/yyyy HH:mm}", fontSmall, XBrushes.DarkGray, leftMargin, yPosition, XStringFormats.TopLeft);
        yPosition += sectionSpacing;

        // --- Información Personal ---
        gfx.DrawString("Información Personal", fontHeader, XBrushes.DarkBlue, leftMargin, yPosition, XStringFormats.TopLeft);
        yPosition += defaultLineHeight + 5;
        DrawField(gfx, "Nombres:", _beneficiario.Nombres ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Apellidos:", _beneficiario.Apellidos ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Fecha de Registro:", _beneficiario.FechaRegistroBeneficiario.ToString("dd/MM/yyyy"), fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Comunidad:", _beneficiario.Comunidad?.NombreComunidad ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Rango de Edad:", _beneficiario.RangoEdad ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Género:", _beneficiario.Genero ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "País de Origen:", $"{_beneficiario.PaisOrigen ?? "N/A"}{(!string.IsNullOrEmpty(_beneficiario.OtroPaisOrigen) ? $" (Otro: {_beneficiario.OtroPaisOrigen})" : "")}", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Estado Migratorio:", $"{_beneficiario.EstadoMigratorio ?? "N/A"}{(!string.IsNullOrEmpty(_beneficiario.OtroEstadoMigratorio) ? $" (Otro: {_beneficiario.OtroEstadoMigratorio})" : "")}", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        DrawField(gfx, "Estado Civil:", _beneficiario.EstadoCivil ?? "N/A", fontBody, XBrushes.Black, leftMargin, ref yPosition, defaultLineHeight, contentWidth);
        yPosition += sectionSpacing;

        // Aquí puedes añadir más secciones y campos de la misma manera.
      }

      using (MemoryStream stream = new MemoryStream())
      {
        document.Save(stream, false);
        return stream.ToArray();
      }
    }

    private void DrawField(XGraphics gfx, string label, string value, XFont font, XBrush brush, double x, ref double y, double minLineHeight, double availableWidth)
    {
      // Dibujar la etiqueta (Línea 94 según tu log para CS1503)
      // Usando la sobrecarga con XPoint para mayor claridad
      XPoint labelPoint = new XPoint(x, y);
      gfx.DrawString(label, font, brush, labelPoint, XStringFormats.TopLeft);

      // Calcular posición y ancho para el valor
      XSize labelSize = gfx.MeasureString(label, font);
      double valueXPosition = x + labelSize.Width + 5;
      double valueWidth = availableWidth - labelSize.Width - 5;

      if (valueWidth <= 10) valueWidth = Math.Max(10, availableWidth / 2);

      XTextFormatter tf = new XTextFormatter(gfx);
      tf.Alignment = XParagraphAlignment.Left;

      XRect valueRect = new XRect(valueXPosition, y, valueWidth, 1000);

      tf.DrawString(value, font, brush, valueRect, XStringFormats.TopLeft);

      // Estimar la altura del texto dibujado por XTextFormatter
      // (Línea 98 según tu log para CS1501)
      // La sobrecarga correcta para MeasureString con ancho restringido es:
      // MeasureString(string text, XFont font, XStringFormat stringFormat, XUnit width)
      // o MeasureString(string text, XFont font, XStringFormat stringFormat, XSize layoutArea)
      // Vamos a usar la que toma el ancho.
      XSize measuredSize = gfx.MeasureString(value, font, XStringFormats.TopLeft, XUnit.FromPoint(valueWidth));

      // (Línea 120 según tu log para CS1501 para GetHeight)
      // font.GetHeight() no toma argumentos. minLineHeight ya usa esto.
      y += Math.Max(minLineHeight, measuredSize.Height);
    }
  }
}
