// VN_Center/Documents/BeneficiarioDetailPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities; // Asegúrate que Beneficiarios y otras entidades estén aquí
using System.Linq;
using System; // Para DateTime

namespace VN_Center.Documents
{
  public class BeneficiarioDetailPdfDocument : IDocument
  {
    private readonly Beneficiarios _beneficiario;
    private readonly string _logoPath;

    public BeneficiarioDetailPdfDocument(Beneficiarios beneficiario, string logoPath)
    {
      _beneficiario = beneficiario;
      _logoPath = logoPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container
          .Page(page =>
          {
            page.Margin(30);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);

            page.Footer().AlignCenter().Text(text =>
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

    void ComposeHeader(IContainer container)
    {
      container.Row(row =>
      {
        if (System.IO.File.Exists(_logoPath))
        {
          row.RelativeItem(1).MaxHeight(70).Image(_logoPath);
        }
        else
        {
          row.RelativeItem(1).MaxHeight(70).Text("Logo no encontrado").Bold();
        }

        row.RelativeItem(3).PaddingLeft(10).Column(column =>
        {
          column.Item().Text($"FICHA DETALLADA DEL BENEFICIARIO")
              .Bold().FontSize(18).FontColor(Colors.Blue.Medium);
          // Corregido: .Trim() aplicado a la cadena
          column.Item().Text(((_beneficiario.Nombres ?? "") + " " + (_beneficiario.Apellidos ?? "")).Trim())
              .SemiBold().FontSize(16);
          column.Item().Text($"ID: {_beneficiario.BeneficiarioID}")
              .FontSize(10);
        });
      });
      container.PaddingBottom(1, Unit.Centimetre);
    }

    void ComposeContent(IContainer container)
    {
      container.PaddingVertical(20).Column(column =>
      {
        column.Spacing(15);

        // SECCIÓN 1: DATOS PERSONALES
        ComposeSectionTitle(column, "1. Datos Personales");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(1); columns.RelativeColumn(2);
            columns.RelativeColumn(1); columns.RelativeColumn(2);
          });
          AddTableRow(table, "Cédula:", _beneficiario.Cedula);
          AddTableRow(table, "Fecha de Nacimiento:", _beneficiario.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "N/A");
          AddTableRow(table, "Rango de Edad:", _beneficiario.RangoEdad ?? "N/A");
          AddTableRow(table, "Género:", _beneficiario.Genero);
          AddTableRow(table, "Nacionalidad:", _beneficiario.Nacionalidad); // Verificar si existe en Beneficiarios.cs
          AddTableRow(table, "Estado Civil:", _beneficiario.EstadoCivil);
          AddTableRow(table, "Lugar de Residencia:", _beneficiario.LugarResidencia); // Verificar si existe en Beneficiarios.cs
          AddTableRow(table, "Dirección Exacta:", _beneficiario.DireccionExacta); // Verificar si existe en Beneficiarios.cs
        });

        // SECCIÓN 2: INFORMACIÓN DE CONTACTO
        ComposeSectionTitle(column, "2. Información de Contacto");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(1); columns.RelativeColumn(2);
            columns.RelativeColumn(1); columns.RelativeColumn(2);
          });
          AddTableRow(table, "Teléfono Principal:", _beneficiario.TelefonoPrincipal); // Verificar si existe
          AddTableRow(table, "Teléfono Secundario:", _beneficiario.TelefonoSecundario ?? "N/A"); // Verificar si existe
          AddTableRow(table, "Correo Electrónico:", _beneficiario.CorreoElectronico); // Verificar si existe
        });

        // SECCIÓN 3: EDUCACIÓN Y EMPLEO
        ComposeSectionTitle(column, "3. Educación y Empleo");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Nivel Educación Completado:", _beneficiario.NivelEducacionCompletado);
          AddTableRow(table, "Ocupación:", _beneficiario.Ocupacion); // Verificar si existe
          AddTableRow(table, "Fuente de Ingresos:", _beneficiario.FuenteIngresos); // Verificar si existe
          AddTableRow(table, "Descripción Otros Ingresos:", _beneficiario.DescripcionOtrosIngresos ?? "N/A"); // Verificar si existe
        });

        // SECCIÓN 4: SITUACIÓN FAMILIAR
        ComposeSectionTitle(column, "4. Situación Familiar");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Jefatura Familiar:", _beneficiario.JefaturaFamiliar); // Verificar si existe
          AddTableRow(table, "Composición Familiar:", _beneficiario.ComposicionFamiliar); // Verificar si existe
          AddTableRow(table, "Nº Personas en el Hogar:", _beneficiario.NumeroPersonasHogar.ToString());
          AddTableRow(table, "Nº Personas Dependientes:", _beneficiario.NumeroPersonasDependientes.ToString()); // Verificar si existe
          AddTableRow(table, "Nº Personas con Discapacidad:", _beneficiario.NumeroPersonasDiscapacidad.ToString()); // Verificar si existe
          AddTableRow(table, "Descripción Discapacidad:", _beneficiario.DescripcionDiscapacidad ?? "N/A"); // Verificar si existe
        });

        // SECCIÓN 5: INTERESES Y NECESIDADES
        ComposeSectionTitle(column, "5. Intereses y Necesidades");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Intereses Vocacionales:", _beneficiario.InteresesVocacionales); // Verificar si existe
          AddTableRow(table, "Necesidades de Capacitación:", _beneficiario.NecesidadesCapacitacion); // Verificar si existe
          AddTableRow(table, "Necesidades de Apoyo:", _beneficiario.NecesidadesApoyo); // Verificar si existe
                                                                                       // Corregido: Nombre de propiedad en FuenteConocimiento
          AddTableRow(table, "Fuente Conocimiento VN:", _beneficiario.FuenteConocimiento?.NombreFuenteConocimiento ?? "N/A");
        });

        // SECCIÓN 6: PROGRAMAS Y PROYECTOS ASOCIADOS
        // Corregido: Nombre de la colección
        if (_beneficiario.BeneficiariosProgramasProyectos != null && _beneficiario.BeneficiariosProgramasProyectos.Any())
        {
          ComposeSectionTitle(column, "6. Programas y Proyectos Vinculados");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3); columns.RelativeColumn(2);
              columns.RelativeColumn(2); columns.RelativeColumn(3);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Programa/Proyecto").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha Inicio").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Estado").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Observaciones").Bold();
            });
            // Corregido: Nombre de la colección
            foreach (var bpp in _beneficiario.BeneficiariosProgramasProyectos)
            {
              // Corregido: Nombre de propiedad en ProgramaProyecto
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bpp.ProgramaProyecto?.NombreProgramaProyecto ?? "N/A");
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bpp.FechaInicio.ToString("dd/MM/yyyy"));
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bpp.EstadoActual);
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bpp.Observaciones ?? "N/A");
            }
          });
        }

        // SECCIÓN 7: GRUPOS COMUNITARIOS ASOCIADOS
        if (_beneficiario.BeneficiarioGrupos != null && _beneficiario.BeneficiarioGrupos.Any())
        {
          ComposeSectionTitle(column, "7. Grupos Comunitarios Vinculados");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3); columns.RelativeColumn(2);
              columns.RelativeColumn(2); columns.RelativeColumn(3);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Grupo Comunitario").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Rol").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha Vinculación").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Observaciones").Bold();
            });
            foreach (var bg in _beneficiario.BeneficiarioGrupos)
            {
              // Corregido: Nombre de propiedad en GrupoComunitario
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bg.GrupoComunitario?.NombreGrupo ?? "N/A");
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bg.RolEnGrupo ?? "N/A");
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bg.FechaVinculacion.ToString("dd/MM/yyyy")); // Verificar si FechaVinculacion existe en BeneficiarioGrupos
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), bg.Observaciones ?? "N/A"); // Verificar si Observaciones existe en BeneficiarioGrupos
            }
          });
        }

        // SECCIÓN 8: ASISTENCIAS RECIBIDAS
        // Corregido: Nombre de la colección
        if (_beneficiario.BeneficiarioAsistenciaRecibida != null && _beneficiario.BeneficiarioAsistenciaRecibida.Any())
        {
          ComposeSectionTitle(column, "8. Asistencias Recibidas");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(5);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Tipo").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Descripción").Bold();
            });
            // Corregido: Nombre de la colección
            foreach (var asistencia in _beneficiario.BeneficiarioAsistenciaRecibida.OrderByDescending(a => a.FechaAsistencia))
            {
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), asistencia.FechaAsistencia.ToString("dd/MM/yyyy"));
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), asistencia.TipoAsistencia);
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), asistencia.Descripcion);
            }
          });
        }

        // SECCIÓN 9: PARTICIPACIONES ACTIVAS
        if (_beneficiario.ParticipacionesActivas != null && _beneficiario.ParticipacionesActivas.Any())
        {
          ComposeSectionTitle(column, "9. Participaciones Activas");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(5);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Actividad").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Detalles").Bold();
            });
            foreach (var participacion in _beneficiario.ParticipacionesActivas.OrderByDescending(p => p.FechaParticipacion))
            {
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), participacion.FechaParticipacion.ToString("dd/MM/yyyy"));
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), participacion.NombreActividad);
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), participacion.Detalles);
            }
          });
        }

        // SECCIÓN 10: EVALUACIONES DE PROGRAMA
        if (_beneficiario.EvaluacionesPrograma != null && _beneficiario.EvaluacionesPrograma.Any())
        {
          ComposeSectionTitle(column, "10. Evaluaciones de Programa");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3); columns.RelativeColumn(2);
              columns.RelativeColumn(1); columns.RelativeColumn(4);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Programa/Proyecto").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha Evaluación").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Calificación").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Comentarios").Bold();
            });
            foreach (var evaluacion in _beneficiario.EvaluacionesPrograma.OrderByDescending(e => e.FechaEvaluacion))
            {
              // Corregido: Nombre de propiedad en ProgramaProyecto
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), evaluacion.ProgramaProyecto?.NombreProgramaProyecto ?? "N/A");
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), evaluacion.FechaEvaluacion.ToString("dd/MM/yyyy"));
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), evaluacion.Calificacion.ToString());
              AddTableRowContent(table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5), evaluacion.Comentarios ?? "N/A");
            }
          });
        }

        // SECCIÓN 11: CONSENTIMIENTO INFORMADO
        ComposeSectionTitle(column, "11. Consentimiento Informado");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Consentimiento Firmado:", _beneficiario.ConsentimientoInformadoFirmado ? "Sí" : "No"); // Verificar si existe
          AddTableRow(table, "Fecha de Firma:", _beneficiario.FechaFirmaConsentimiento?.ToString("dd/MM/yyyy") ?? "N/A"); // Verificar si existe
          AddTableRow(table, "Observaciones Consentimiento:", _beneficiario.ObservacionesConsentimiento ?? "N/A"); // Verificar si existe
        });

        // SECCIÓN 12: ESTADO DEL BENEFICIARIO
        ComposeSectionTitle(column, "12. Estado del Beneficiario");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Activo:", _beneficiario.Activo ? "Sí" : "No"); // Verificar si existe
          AddTableRow(table, "Fecha de Registro:", _beneficiario.FechaRegistroBeneficiario.ToString("dd/MM/yyyy HH:mm"));
          if (_beneficiario.FechaActualizacion.HasValue) // Verificar si existe
          {
            AddTableRow(table, "Última Actualización:", _beneficiario.FechaActualizacion.Value.ToString("dd/MM/yyyy HH:mm"));
          }
          AddTableRow(table, "Usuario que registró:", _beneficiario.UsuarioRegistroId ?? "N/A"); // Verificar si existe
          AddTableRow(table, "Observaciones Generales:", _beneficiario.ObservacionesGenerales ?? "N/A"); // Verificar si existe
        });
      });
    }

    void ComposeSectionTitle(ColumnDescriptor column, string title)
    {
      column.Item().PaddingTop(10).Text(title).Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
      column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
      column.Item().PaddingBottom(5);
    }

    void AddTableRow(TableDescriptor table, string label, string? value)
    {
      table.Cell().Padding(2).Text(label).SemiBold();
      table.Cell().Padding(2).Text(value ?? "N/A");
    }

    void AddTableRow(TableDescriptor table, string label1, string? value1, string label2, string? value2)
    {
      table.Cell().Padding(2).Text(label1).SemiBold();
      table.Cell().Padding(2).Text(value1 ?? "N/A");
      table.Cell().Padding(2).Text(label2).SemiBold();
      table.Cell().Padding(2).Text(value2 ?? "N/A");
    }

    // Corregido para intentar solucionar CS1929
    void AddTableRowContent(TableCellDescriptor cell, string? value)
    {
      cell.Text(text => text.Span(value ?? "N/A"));
    }
  }
}
