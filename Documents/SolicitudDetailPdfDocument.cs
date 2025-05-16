// VN_Center/Documents/SolicitudDetailPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities;
using System;
using System.Linq; // Si se usa para colecciones

namespace VN_Center.Documents
{
  public class SolicitudDetailPdfDocument : IDocument
  {
    private readonly Solicitudes _solicitud;
    private readonly string _logoPath;

    // Helpers para formateo
    private Func<string?, string> DisplayString => s => string.IsNullOrWhiteSpace(s) ? "No especificado" : s;
    private Func<DateTime?, string> DisplayDate => d => d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "No especificada";
    private Func<bool?, string> DisplayBoolean => b => b.HasValue ? (b.Value ? "Sí" : "No") : "No especificado";
    private Func<int?, string> DisplayIntRating => i => i.HasValue ? $"{i.Value}/5" : "No especificado";


    public SolicitudDetailPdfDocument(Solicitudes solicitud, string logoPath)
    {
      _solicitud = solicitud;
      _logoPath = logoPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(30); // Margen general de la página

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
              row.RelativeItem(1).MaxHeight(65); // Espacio reservado
            }

            row.RelativeItem(4).PaddingLeft(10).Column(column =>
            {
              column.Item().Text($"Detalle de Solicitud Nro: {_solicitud.SolicitudID}")
                        .Bold().FontSize(16).FontColor(Colors.Blue.Medium);
              column.Item().Text($"Solicitante: {_solicitud.Nombres} {_solicitud.Apellidos}")
                        .SemiBold().FontSize(12);
              column.Item().Text($"Tipo: {_solicitud.TipoSolicitud} | Estado: {_solicitud.EstadoSolicitud}")
                        .FontSize(10);
              column.Item().Text($"Fecha de Envío: {_solicitud.FechaEnvioSolicitud:dd/MM/yyyy HH:mm}")
                        .FontSize(9);
            });
          });
    }

    void ComposeContent(IContainer container)
    {
      container.PaddingVertical(10).Column(column =>
      {
        column.Spacing(12);

        // Sección: Información Personal y de Contacto
        ComposeSectionTitle(column, "Información Personal y de Contacto");
        AddDetailRow(column, "Nombres:", _solicitud.Nombres);
        AddDetailRow(column, "Apellidos:", _solicitud.Apellidos);
        AddDetailRow(column, "Correo Electrónico:", _solicitud.Email);
        AddDetailRow(column, "Teléfono:", DisplayString(_solicitud.Telefono));
        AddDetailRow(column, "Permite Contacto por WhatsApp:", DisplayBoolean(_solicitud.PermiteContactoWhatsApp));
        AddDetailRow(column, "Fecha de Nacimiento:", DisplayDate(_solicitud.FechaNacimiento));
        AddDetailRow(column, "Dirección:", DisplayString(_solicitud.Direccion), true);

        // Sección: Detalles de la Solicitud y Documentación
        ComposeSectionTitle(column, "Detalles de la Solicitud y Documentación");
        AddDetailRow(column, "Tipo de Solicitud:", _solicitud.TipoSolicitud);
        AddDetailRow(column, "Fecha de Inicio Preferida:", DisplayDate(_solicitud.FechaInicioPreferida));
        AddDetailRow(column, "Duración del Programa:", DisplayString(_solicitud.DuracionEstancia));
        if (!string.IsNullOrWhiteSpace(_solicitud.DuracionEstanciaOtro))
          AddDetailRow(column, "Otra Duración:", _solicitud.DuracionEstanciaOtro);
        AddDetailRow(column, "Pasaporte Válido (+6 meses):", DisplayBoolean(_solicitud.PasaporteValidoSeisMeses));
        AddDetailRow(column, "Fecha Expiración Pasaporte:", DisplayDate(_solicitud.FechaExpiracionPasaporte));
        AddDetailRow(column, "Profesión u Ocupación:", DisplayString(_solicitud.ProfesionOcupacion));
        AddDetailRow(column, "Estado de la Solicitud:", _solicitud.EstadoSolicitud);
        // PathCV y PathCartaRecomendacion se omiten ya que son rutas, no contenido directo para el PDF.

        // Sección: Experiencia, Habilidades e Idiomas
        ComposeSectionTitle(column, "Experiencia, Habilidades e Idiomas");
        AddDetailRow(column, "Experiencia Previa (Voluntariado/Pasantía/Servicio):", DisplayString(_solicitud.ExperienciaPreviaSVL), true);
        AddDetailRow(column, "Experiencia Laboral Relevante:", DisplayString(_solicitud.ExperienciaLaboralRelevante), true);
        AddDetailRow(column, "Habilidades Relevantes:", DisplayString(_solicitud.HabilidadesRelevantes), true);
        AddDetailRow(column, "Nivel de Español:", _solicitud.NivelesIdioma?.NombreNivel ?? "No especificado");
        AddDetailRow(column, "Comodidad con Habilidades en Español (1-5):", DisplayIntRating(_solicitud.ComodidadHabilidadesEsp));
        AddDetailRow(column, "Otros Idiomas Hablados:", DisplayString(_solicitud.OtrosIdiomasHablados));
        AddDetailRow(column, "Años de Entrenamiento Formal en Español:", DisplayString(_solicitud.AniosEntrenamientoFormalEsp));


        // Sección: Motivaciones y Conocimiento del Programa
        ComposeSectionTitle(column, "Motivaciones y Conocimiento del Programa");
        AddDetailRow(column, "Motivación e Interés en Costa Rica:", DisplayString(_solicitud.MotivacionInteresCR), true);
        AddDetailRow(column, "Descripción: Salir de tu Zona de Confort:", DisplayString(_solicitud.DescripcionSalidaZonaConfort), true);
        AddDetailRow(column, "Información Adicional Personal:", DisplayString(_solicitud.InformacionAdicionalPersonal), true);
        AddDetailRow(column, "¿Cómo nos conoció?:", _solicitud.FuentesConocimiento?.NombreFuente ?? "No especificado");
        if (!string.IsNullOrWhiteSpace(_solicitud.FuenteConocimientoOtro))
          AddDetailRow(column, "Otra Fuente:", _solicitud.FuenteConocimientoOtro);

        // Sección: Información Específica de Pasantías (si aplica)
        if (_solicitud.TipoSolicitud == "Pasantia")
        {
          ComposeSectionTitle(column, "Información Específica de Pasantías");
          AddDetailRow(column, "Nombre de Universidad:", DisplayString(_solicitud.NombreUniversidad));
          AddDetailRow(column, "Carrera / Área de Estudio:", DisplayString(_solicitud.CarreraAreaEstudio));
          AddDetailRow(column, "Fecha de Graduación (o esperada):", DisplayDate(_solicitud.FechaGraduacionEsperada));
          AddDetailRow(column, "Preferencias de Alojamiento en Familia:", DisplayString(_solicitud.PreferenciasAlojamientoFamilia), true);
          AddDetailRow(column, "Ensayo: Relación con Estudios/Intereses:", DisplayString(_solicitud.EnsayoRelacionEstudiosIntereses), true);
          AddDetailRow(column, "Ensayo: Habilidades/Conocimientos a Adquirir:", DisplayString(_solicitud.EnsayoHabilidadesConocimientosAdquirir), true);
          AddDetailRow(column, "Ensayo: Contribución y Aprendizaje en Comunidad Anfitriona:", DisplayString(_solicitud.EnsayoContribucionAprendizajeComunidad), true);
          AddDetailRow(column, "Ensayo: Experiencias Transculturales Previas:", DisplayString(_solicitud.EnsayoExperienciasTransculturalesPrevias), true);
        }

        // Sección: Contacto de Emergencia e Información Adicional
        ComposeSectionTitle(column, "Contacto de Emergencia e Información Adicional");
        AddDetailRow(column, "Nombre Contacto de Emergencia:", DisplayString(_solicitud.NombreContactoEmergencia));
        AddDetailRow(column, "Relación Contacto de Emergencia:", DisplayString(_solicitud.RelacionContactoEmergencia));
        AddDetailRow(column, "Teléfono Contacto de Emergencia:", DisplayString(_solicitud.TelefonoContactoEmergencia));
        AddDetailRow(column, "Email Contacto de Emergencia:", DisplayString(_solicitud.EmailContactoEmergencia));
        AddDetailRow(column, "Información Personal (familia, hobbies, etc.):", DisplayString(_solicitud.InfoPersonalFamiliaHobbies), true);
        AddDetailRow(column, "Alergias o Restricciones Dietéticas:", DisplayString(_solicitud.AlergiasRestriccionesDieteticas), true);
        AddDetailRow(column, "Solicitudes Especiales para Alojamiento:", DisplayString(_solicitud.SolicitudesEspecialesAlojamiento), true);
      });
    }

    void ComposeSectionTitle(ColumnDescriptor column, string title)
    {
      column.Item().PaddingTop(10).Text(title).Bold().FontSize(13).FontColor(Colors.Grey.Darken2);
      column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
      column.Item().PaddingBottom(4);
    }

    void AddDetailRow(ColumnDescriptor column, string label, string? value, bool isMultiline = false)
    {
      column.Item().Row(row =>
      {
        row.RelativeItem(1).PaddingRight(5).Text(label).SemiBold().FontSize(9);

        var valueCell = row.RelativeItem(2);
        if (isMultiline && !string.IsNullOrEmpty(value) && value.Contains(Environment.NewLine)) // Usar Environment.NewLine para saltos de línea
        {
          var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
          valueCell.Column(colValue => {
            foreach (var line in lines)
            {
              colValue.Item().Text(line).FontSize(9);
            }
          });
        }
        else
        {
          valueCell.Text(value ?? "No especificado").FontSize(9);
        }
      });
      column.Item().PaddingBottom(3);
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
