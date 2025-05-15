// VN_Center/Documents/EvaluacionProgramaDetailPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities;
using System;
using System.Linq; // Necesario para Any() si se usa en colecciones, aunque aquí no tanto.

namespace VN_Center.Documents
{
  public class EvaluacionProgramaDetailPdfDocument : IDocument
  {
    private readonly EvaluacionesPrograma _evaluacion;
    private readonly string _logoPath;

    // Funciones auxiliares para mostrar valores o "No especificado"
    private Func<string?, string> DisplayString => s => string.IsNullOrWhiteSpace(s) ? "No especificado" : s;
    private Func<int?, string> DisplayIntRating => i => i.HasValue ? $"{i.Value}/5" : "No especificado";
    private Func<bool?, string> DisplayBoolean => b => b.HasValue ? (b.Value ? "Sí" : "No") : "No especificado";


    public EvaluacionProgramaDetailPdfDocument(EvaluacionesPrograma evaluacion, string logoPath)
    {
      _evaluacion = evaluacion;
      _logoPath = logoPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(30); // Margen reducido para más contenido

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
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
              row.RelativeItem(1).MaxHeight(65).Image(_logoPath);
            }
            else
            {
              row.RelativeItem(1).MaxHeight(65); // Espacio para el logo
            }

            row.RelativeItem(4).PaddingLeft(10).Column(column =>
            {
              column.Item().Text($"Detalle de Evaluación de Programa ID: {_evaluacion.EvaluacionID}")
                        .Bold().FontSize(16).FontColor(Colors.Blue.Medium);

              string participanteNombre = "Participante Desconocido";
              if (_evaluacion.ParticipacionActiva?.Solicitud != null)
              {
                participanteNombre = $"{_evaluacion.ParticipacionActiva.Solicitud.Nombres} {_evaluacion.ParticipacionActiva.Solicitud.Apellidos}".Trim();
              }
              column.Item().Text($"Participante: {participanteNombre}").SemiBold().FontSize(11);

              string programaNombre = _evaluacion.ParticipacionActiva?.ProgramaProyecto?.NombreProgramaProyecto ?? "N/A";
              column.Item().Text($"Programa: {programaNombre}").FontSize(10);
              column.Item().Text($"Fecha de Evaluación: {_evaluacion.FechaEvaluacion:dd/MM/yyyy HH:mm}").FontSize(9);
            });
          });
    }

    void ComposeContent(IContainer container)
    {
      container.PaddingVertical(10).Column(column => // Reducido el padding vertical
      {
        column.Spacing(12); // Reducido el espaciado

        // Sección: Sobre el Programa
        ComposeSectionTitle(column, "Sobre el Programa");
        AddDetailRow(column, "Nombre del Programa/Universidad del Evaluador:", DisplayString(_evaluacion.NombreProgramaUniversidadEvaluador));
        AddDetailRow(column, "Parte Más Gratificante del Programa:", DisplayString(_evaluacion.ParteMasGratificante), true);
        AddDetailRow(column, "Parte Más Difícil del Programa:", DisplayString(_evaluacion.ParteMasDificil), true);
        AddDetailRow(column, "Razones Originales para Participar:", DisplayString(_evaluacion.RazonesOriginalesParticipacion), true);
        AddDetailRow(column, "¿Se Cumplieron tus Expectativas Originales?:", DisplayString(_evaluacion.ExpectativasOriginalesCumplidas));
        AddDetailRow(column, "Utilidad de Información Previa (1-5):", DisplayIntRating(_evaluacion.InformacionPreviaUtil));

        // Sección: Experiencia Cultural y Actividades
        ComposeSectionTitle(column, "Experiencia Cultural y Actividades");
        AddDetailRow(column, "Esfuerzo de Integración en Comunidades:", DisplayString(_evaluacion.EsfuerzoIntegracionComunidades));
        AddDetailRow(column, "Comentarios sobre Alojamiento/Hotel:", DisplayString(_evaluacion.ComentariosAlojamientoHotel), true);
        AddDetailRow(column, "Inmersión Cultural y Humildad (1-5):", DisplayIntRating(_evaluacion.ProgramaInmersionCulturalAyudoHumildad));
        AddDetailRow(column, "Actividades Recreativas/Culturales Interesantes:", DisplayString(_evaluacion.ActividadesRecreativasCulturalesInteresantes));
        AddDetailRow(column, "Visita de Sitio/Comunidad Favorita y Por Qué:", DisplayString(_evaluacion.VisitaSitioComunidadFavoritaYPorQue), true);

        // Sección: Aprendizaje y Recomendaciones
        ComposeSectionTitle(column, "Aprendizaje y Recomendaciones");
        AddDetailRow(column, "Aspecto Más Valioso de la Experiencia:", DisplayString(_evaluacion.AspectoMasValiosoExperiencia), true);
        AddDetailRow(column, "Aplicaré lo Aprendido (1-5):", DisplayIntRating(_evaluacion.AplicaraLoAprendidoFuturo));
        AddDetailRow(column, "Tres Cosas que Aprendí Sobre Mí Mismo:", DisplayString(_evaluacion.TresCosasAprendidasSobreSiMismo), true);
        AddDetailRow(column, "¿Cómo Compartiré lo Aprendido en mi Universidad/Comunidad?:", DisplayString(_evaluacion.ComoCompartiraAprendidoUniversidad), true);
        AddDetailRow(column, "¿Recomendarías este Programa a Otros?:", DisplayString(_evaluacion.RecomendariaProgramaOtros));
        AddDetailRow(column, "¿Qué Dirías a Otros sobre el Programa?:", DisplayString(_evaluacion.QueDiraOtrosSobrePrograma), true);
        AddDetailRow(column, "¿Podemos Usarte como Referencia?:", DisplayBoolean(_evaluacion.PermiteSerUsadoComoReferencia));
        AddDetailRow(column, "Comentarios Adicionales:", DisplayString(_evaluacion.ComentariosAdicionalesEvaluacion), true);

      });
    }

    void ComposeSectionTitle(ColumnDescriptor column, string title)
    {
      column.Item().PaddingTop(8).Text(title).Bold().FontSize(13).FontColor(Colors.Blue.Darken1); // Tamaño reducido
      column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium); // Línea más delgada
      column.Item().PaddingBottom(3);
    }

    void AddDetailRow(ColumnDescriptor column, string label, string? value, bool isMultiline = false)
    {
      column.Item().Row(row =>
      {
        row.RelativeItem(1).Text(label).SemiBold().FontSize(9); // Tamaño reducido
        if (isMultiline && !string.IsNullOrEmpty(value) && value.Contains("\n"))
        {
          // Si es multilínea y contiene saltos, procesar cada línea
          var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
          row.RelativeItem(2).Column(colValue => {
            foreach (var line in lines)
            {
              colValue.Item().Text(line).FontSize(9); // Tamaño reducido
            }
          });
        }
        else
        {
          row.RelativeItem(2).Text(value ?? "No especificado").FontSize(9); // Tamaño reducido
        }
      });
      column.Item().PaddingBottom(2); // Espacio reducido entre filas
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
  }
}
