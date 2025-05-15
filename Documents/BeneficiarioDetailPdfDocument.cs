// VN_Center/Documents/BeneficiarioDetailPdfDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VN_Center.Models.Entities;
using System.Linq;
using System;

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
            // La llamada a Element para el Header es correcta
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
      // Aplicar PaddingBottom PRIMERO y luego añadir el Row como contenido de ese contenedor acolchado.
      container
          .PaddingBottom(1, Unit.Centimetre) // Aplicar el padding al contenedor principal del header
          .Row(row => // El Row es ahora el contenido del contenedor que ya tiene PaddingBottom
          {
            // Logo
            if (System.IO.File.Exists(_logoPath))
            {
              row.RelativeItem(1).MaxHeight(70).Image(_logoPath);
            }
            else
            {
              row.RelativeItem(1).MaxHeight(70).Text("Logo no encontrado").Bold();
            }

            // Título
            row.RelativeItem(3).PaddingLeft(10).Column(column =>
            {
              column.Item().Text("FICHA DETALLADA DEL BENEFICIARIO")
                        .Bold().FontSize(18).FontColor(Colors.Blue.Medium);
              column.Item().Text(_beneficiario.NombreCompleto)
                        .SemiBold().FontSize(16);
              column.Item().Text($"ID: {_beneficiario.BeneficiarioID}")
                        .FontSize(10);
            });
          });
      // Ya no se necesita el container.PaddingBottom(1, Unit.Centimetre); aquí abajo porque se encadenó arriba.
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
          AddTableRow(table, "Nombres:", _beneficiario.Nombres);
          AddTableRow(table, "Apellidos:", _beneficiario.Apellidos);
          AddTableRow(table, "Rango de Edad:", _beneficiario.RangoEdad);
          AddTableRow(table, "Género:", _beneficiario.Genero);
          AddTableRow(table, "País de Origen:", _beneficiario.PaisOrigen);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroPaisOrigen))
            AddTableRow(table, "Otro País de Origen:", _beneficiario.OtroPaisOrigen);
          AddTableRow(table, "Estado Migratorio:", _beneficiario.EstadoMigratorio);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroEstadoMigratorio))
            AddTableRow(table, "Otro Estado Migratorio:", _beneficiario.OtroEstadoMigratorio);
          AddTableRow(table, "Estado Civil:", _beneficiario.EstadoCivil);
        });

        // SECCIÓN 3: EDUCACIÓN Y EMPLEO
        ComposeSectionTitle(column, "3. Educación y Empleo");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Nivel Educación Completado:", _beneficiario.NivelEducacionCompletado);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroNivelEducacion))
            AddTableRow(table, "Otro Nivel Educación:", _beneficiario.OtroNivelEducacion);
          AddTableRow(table, "Está Empleado Personalmente:", _beneficiario.EstaEmpleadoPersonalmente);
          AddTableRow(table, "Tipo Situación Laboral:", _beneficiario.TipoSituacionLaboral);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroTipoSituacionLaboral))
            AddTableRow(table, "Otra Situación Laboral:", _beneficiario.OtroTipoSituacionLaboral);
          AddTableRow(table, "Tipo Trabajo Realizado (si empleado):", _beneficiario.TipoTrabajoRealizadoSiEmpleado);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroTipoTrabajoRealizado))
            AddTableRow(table, "Otro Tipo Trabajo:", _beneficiario.OtroTipoTrabajoRealizado);
        });

        // SECCIÓN 4: SITUACIÓN FAMILIAR Y VIVIENDA
        ComposeSectionTitle(column, "4. Situación Familiar y Vivienda");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Nº Personas en el Hogar:", _beneficiario.NumeroPersonasHogar?.ToString() ?? "N/A");
          AddTableRow(table, "Vivienda Alquilada o Propia:", _beneficiario.ViviendaAlquiladaOPropia);
          AddTableRow(table, "Miembros del Hogar Empleados:", _beneficiario.MiembrosHogarEmpleados?.ToString() ?? "N/A");
          AddTableRow(table, "Tiempo en Costa Rica (si migrante):", _beneficiario.TiempoEnCostaRicaSiMigrante);
          AddTableRow(table, "Tiempo Viviendo en Comunidad Actual:", _beneficiario.TiempoViviendoEnComunidadActual);
        });

        // SECCIÓN 5: NECESIDADES Y PERCEPCIONES
        ComposeSectionTitle(column, "5. Necesidades y Percepciones");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Ingresos Suficientes para Necesidades:", _beneficiario.IngresosSuficientesNecesidades);
          AddTableRow(table, "Inscrito Prog. Educación/Capacitación Actual:", _beneficiario.InscritoProgramaEducacionCapacitacionActual);
          AddTableRow(table, "Niños en Hogar Asisten Escuela:", _beneficiario.NinosHogarAsistenEscuela);
          AddTableRow(table, "Barreras Asistencia Escolar Niños:", _beneficiario.BarrerasAsistenciaEscolarNinos);
          if (!string.IsNullOrWhiteSpace(_beneficiario.OtroBarrerasAsistenciaEscolar))
            AddTableRow(table, "Otras Barreras Escolares:", _beneficiario.OtroBarrerasAsistenciaEscolar);
          AddTableRow(table, "Percepción Acceso Igual Oport. Laborales Mujeres:", _beneficiario.PercepcionAccesoIgualOportunidadesLaboralesMujeres);
          AddTableRow(table, "Disponibilidad Servicios Mujeres Víctimas Violencia:", _beneficiario.DisponibilidadServiciosMujeresVictimasViolencia);
          AddTableRow(table, "Disponibilidad Servicios Salud Mujer:", _beneficiario.DisponibilidadServiciosSaludMujer);
          AddTableRow(table, "Disponibilidad Servicios Apoyo Adultos Mayores:", _beneficiario.DisponibilidadServiciosApoyoAdultosMayores);
        });

        // SECCIÓN 6: ACCESO A SERVICIOS Y TECNOLOGÍA
        ComposeSectionTitle(column, "6. Acceso a Servicios y Tecnología");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Accesibilidad Servicios Transporte Comunidad:", _beneficiario.AccesibilidadServiciosTransporteComunidad);
          AddTableRow(table, "Acceso a Computadora:", _beneficiario.AccesoComputadora);
          AddTableRow(table, "Acceso a Internet:", _beneficiario.AccesoInternet);
        });

        // SECCIÓN 7: PROGRAMAS Y PROYECTOS ASOCIADOS
        if (_beneficiario.BeneficiariosProgramasProyectos != null && _beneficiario.BeneficiariosProgramasProyectos.Any())
        {
          ComposeSectionTitle(column, "7. Programas y Proyectos Vinculados");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3); columns.RelativeColumn(2);
              columns.RelativeColumn(2); columns.RelativeColumn(3);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Programa/Proyecto").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha Inscripción").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Estado Participación").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Notas").Bold();
            });
            foreach (var bpp in _beneficiario.BeneficiariosProgramasProyectos)
            {
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bpp.ProgramaProyecto?.NombreProgramaProyecto ?? "N/A"));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bpp.FechaInscripcionBeneficiario.ToString("dd/MM/yyyy")));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bpp.EstadoParticipacionBeneficiario));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bpp.NotasAdicionales ?? "N/A"));
            }
          });
        }

        // SECCIÓN 8: GRUPOS COMUNITARIOS ASOCIADOS
        if (_beneficiario.BeneficiarioGrupos != null && _beneficiario.BeneficiarioGrupos.Any())
        {
          ComposeSectionTitle(column, "8. Grupos Comunitarios Vinculados");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3); columns.RelativeColumn(2);
              columns.RelativeColumn(2);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Grupo Comunitario").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Rol").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha Unión").Bold();
            });
            foreach (var bg in _beneficiario.BeneficiarioGrupos)
            {
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bg.GrupoComunitario?.NombreGrupo ?? "N/A"));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bg.RolEnGrupo ?? "N/A"));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(bg.FechaUnionGrupo?.ToString("dd/MM/yyyy") ?? "N/A"));
            }
          });
        }

        // SECCIÓN 9: ASISTENCIAS RECIBIDAS
        if (_beneficiario.BeneficiarioAsistenciaRecibida != null && _beneficiario.BeneficiarioAsistenciaRecibida.Any())
        {
          ComposeSectionTitle(column, "9. Asistencias Recibidas");
          column.Item().Table(table =>
          {
            table.ColumnsDefinition(columns => {
              columns.RelativeColumn(3);
              columns.RelativeColumn(5);
            });
            table.Header(header => {
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Tipo Asistencia").Bold();
              header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Notas Adicionales").Bold();
            });
            foreach (var asistencia in _beneficiario.BeneficiarioAsistenciaRecibida)
            {
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(asistencia.TipoAsistencia?.NombreAsistencia ?? "N/A"));
              table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(text => text.Span(asistencia.NotasAdicionales));
            }
          });
        }

        // SECCIÓN 10: ESTADO DEL BENEFICIARIO
        ComposeSectionTitle(column, "10. Estado del Beneficiario");
        column.Item().Table(table =>
        {
          table.ColumnsDefinition(columns => { columns.RelativeColumn(1); columns.RelativeColumn(2); });
          AddTableRow(table, "Fecha de Registro:", _beneficiario.FechaRegistroBeneficiario.ToString("dd/MM/yyyy HH:mm"));
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
  }
}
