using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VN_Center.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTablesAndOtherChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescripcionRol = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaUltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CamposInteresVocacional",
                columns: table => new
                {
                    CampoInteresID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCampo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescripcionCampo = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamposInteresVocacional", x => x.CampoInteresID);
                });

            migrationBuilder.CreateTable(
                name: "Comunidades",
                columns: table => new
                {
                    ComunidadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreComunidad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UbicacionDetallada = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    NotasComunidad = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunidades", x => x.ComunidadID);
                });

            migrationBuilder.CreateTable(
                name: "FuentesConocimiento",
                columns: table => new
                {
                    FuenteConocimientoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreFuente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuentesConocimiento", x => x.FuenteConocimientoID);
                });

            migrationBuilder.CreateTable(
                name: "NivelesIdioma",
                columns: table => new
                {
                    NivelIdiomaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreNivel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesIdioma", x => x.NivelIdiomaID);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    PermisoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePermiso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescripcionPermiso = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.PermisoID);
                });

            migrationBuilder.CreateTable(
                name: "TiposAsistencia",
                columns: table => new
                {
                    TipoAsistenciaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreAsistencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAsistencia", x => x.TipoAsistenciaID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramasProyectosONG",
                columns: table => new
                {
                    ProgramaProyectoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProgramaProyecto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    TipoIniciativa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInicioEstimada = table.Column<DateTime>(type: "DATE", nullable: true),
                    FechaFinEstimada = table.Column<DateTime>(type: "DATE", nullable: true),
                    FechaInicioReal = table.Column<DateTime>(type: "DATE", nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "DATE", nullable: true),
                    EstadoProgramaProyecto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponsablePrincipalONGUsuarioID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasProyectosONG", x => x.ProgramaProyectoID);
                    table.ForeignKey(
                        name: "FK_ProgramasProyectosONG_AspNetUsers_ResponsablePrincipalONGUsuarioID",
                        column: x => x.ResponsablePrincipalONGUsuarioID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    BeneficiarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaRegistroBeneficiario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComunidadID = table.Column<int>(type: "int", nullable: true),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RangoEdad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PaisOrigen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OtroPaisOrigen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstadoMigratorio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OtroEstadoMigratorio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumeroPersonasHogar = table.Column<int>(type: "int", nullable: true),
                    ViviendaAlquiladaOPropia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MiembrosHogarEmpleados = table.Column<int>(type: "int", nullable: true),
                    EstaEmpleadoPersonalmente = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    TipoSituacionLaboral = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OtroTipoSituacionLaboral = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoTrabajoRealizadoSiEmpleado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OtroTipoTrabajoRealizado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TiempoEnCostaRicaSiMigrante = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TiempoViviendoEnComunidadActual = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IngresosSuficientesNecesidades = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NivelEducacionCompletado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OtroNivelEducacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InscritoProgramaEducacionCapacitacionActual = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NinosHogarAsistenEscuela = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BarrerasAsistenciaEscolarNinos = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    OtroBarrerasAsistenciaEscolar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PercepcionAccesoIgualOportunidadesLaboralesMujeres = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DisponibilidadServiciosMujeresVictimasViolencia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DisponibilidadServiciosSaludMujer = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DisponibilidadServiciosApoyoAdultosMayores = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AccesibilidadServiciosTransporteComunidad = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    AccesoComputadora = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AccesoInternet = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.BeneficiarioID);
                    table.ForeignKey(
                        name: "FK_Beneficiarios_Comunidades_ComunidadID",
                        column: x => x.ComunidadID,
                        principalTable: "Comunidades",
                        principalColumn: "ComunidadID");
                });

            migrationBuilder.CreateTable(
                name: "GruposComunitarios",
                columns: table => new
                {
                    GrupoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreGrupo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescripcionGrupo = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ComunidadID = table.Column<int>(type: "int", nullable: true),
                    TipoGrupo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PersonaContactoPrincipal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TelefonoContactoGrupo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmailContactoGrupo = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposComunitarios", x => x.GrupoID);
                    table.ForeignKey(
                        name: "FK_GruposComunitarios_Comunidades_ComunidadID",
                        column: x => x.ComunidadID,
                        principalTable: "Comunidades",
                        principalColumn: "ComunidadID");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesInformacionGeneral",
                columns: table => new
                {
                    SolicitudInfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailContacto = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PermiteContactoWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    ProgramaDeInteres = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProgramaDeInteresOtro = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PreguntasEspecificas = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    EstadoSolicitudInfo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NotasSeguimiento = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    UsuarioAsignadoID = table.Column<int>(type: "int", nullable: true),
                    FuenteConocimientoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesInformacionGeneral", x => x.SolicitudInfoID);
                    table.ForeignKey(
                        name: "FK_SolicitudesInformacionGeneral_AspNetUsers_UsuarioAsignadoID",
                        column: x => x.UsuarioAsignadoID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesInformacionGeneral_FuentesConocimiento_FuenteConocimientoID",
                        column: x => x.FuenteConocimientoID,
                        principalTable: "FuentesConocimiento",
                        principalColumn: "FuenteConocimientoID");
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    SolicitudID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PermiteContactoWhatsApp = table.Column<bool>(type: "bit", nullable: true),
                    Direccion = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "DATE", nullable: true),
                    TipoSolicitud = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaEnvioSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasaporteValidoSeisMeses = table.Column<bool>(type: "bit", nullable: true),
                    FechaExpiracionPasaporte = table.Column<DateTime>(type: "DATE", nullable: true),
                    ProfesionOcupacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NivelIdiomaEspañolID = table.Column<int>(type: "int", nullable: true),
                    OtrosIdiomasHablados = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExperienciaPreviaSVL = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ExperienciaLaboralRelevante = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    HabilidadesRelevantes = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    FechaInicioPreferida = table.Column<DateTime>(type: "DATE", nullable: true),
                    DuracionEstancia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DuracionEstanciaOtro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotivacionInteresCR = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    DescripcionSalidaZonaConfort = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    InformacionAdicionalPersonal = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    FuenteConocimientoID = table.Column<int>(type: "int", nullable: true),
                    FuenteConocimientoOtro = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PathCV = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PathCartaRecomendacion = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EstadoSolicitud = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreUniversidad = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CarreraAreaEstudio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaGraduacionEsperada = table.Column<DateTime>(type: "DATE", nullable: true),
                    PreferenciasAlojamientoFamilia = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    EnsayoRelacionEstudiosIntereses = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    EnsayoHabilidadesConocimientosAdquirir = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    EnsayoContribucionAprendizajeComunidad = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    EnsayoExperienciasTransculturalesPrevias = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    NombreContactoEmergencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TelefonoContactoEmergencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmailContactoEmergencia = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    RelacionContactoEmergencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AniosEntrenamientoFormalEsp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ComodidadHabilidadesEsp = table.Column<int>(type: "int", nullable: true),
                    InfoPersonalFamiliaHobbies = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AlergiasRestriccionesDieteticas = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    SolicitudesEspecialesAlojamiento = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.SolicitudID);
                    table.ForeignKey(
                        name: "FK_Solicitudes_FuentesConocimiento_FuenteConocimientoID",
                        column: x => x.FuenteConocimientoID,
                        principalTable: "FuentesConocimiento",
                        principalColumn: "FuenteConocimientoID");
                    table.ForeignKey(
                        name: "FK_Solicitudes_NivelesIdioma_NivelIdiomaEspañolID",
                        column: x => x.NivelIdiomaEspañolID,
                        principalTable: "NivelesIdioma",
                        principalColumn: "NivelIdiomaID");
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    RolUsuarioID = table.Column<int>(type: "int", nullable: false),
                    PermisoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => new { x.RolUsuarioID, x.PermisoID });
                    table.ForeignKey(
                        name: "FK_RolPermisos_AspNetRoles_RolUsuarioID",
                        column: x => x.RolUsuarioID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisoID",
                        column: x => x.PermisoID,
                        principalTable: "Permisos",
                        principalColumn: "PermisoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaProyectoComunidades",
                columns: table => new
                {
                    ProgramaProyectoID = table.Column<int>(type: "int", nullable: false),
                    ComunidadID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaProyectoComunidades", x => new { x.ProgramaProyectoID, x.ComunidadID });
                    table.ForeignKey(
                        name: "FK_ProgramaProyectoComunidades_Comunidades_ComunidadID",
                        column: x => x.ComunidadID,
                        principalTable: "Comunidades",
                        principalColumn: "ComunidadID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramaProyectoComunidades_ProgramasProyectosONG_ProgramaProyectoID",
                        column: x => x.ProgramaProyectoID,
                        principalTable: "ProgramasProyectosONG",
                        principalColumn: "ProgramaProyectoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeneficiarioAsistenciaRecibida",
                columns: table => new
                {
                    BeneficiarioID = table.Column<int>(type: "int", nullable: false),
                    TipoAsistenciaID = table.Column<int>(type: "int", nullable: false),
                    NotasAdicionales = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiarioAsistenciaRecibida", x => new { x.BeneficiarioID, x.TipoAsistenciaID });
                    table.ForeignKey(
                        name: "FK_BeneficiarioAsistenciaRecibida_Beneficiarios_BeneficiarioID",
                        column: x => x.BeneficiarioID,
                        principalTable: "Beneficiarios",
                        principalColumn: "BeneficiarioID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficiarioAsistenciaRecibida_TiposAsistencia_TipoAsistenciaID",
                        column: x => x.TipoAsistenciaID,
                        principalTable: "TiposAsistencia",
                        principalColumn: "TipoAsistenciaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeneficiariosProgramasProyectos",
                columns: table => new
                {
                    BeneficiarioID = table.Column<int>(type: "int", nullable: false),
                    ProgramaProyectoID = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcionBeneficiario = table.Column<DateTime>(type: "DATE", nullable: false),
                    EstadoParticipacionBeneficiario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NotasAdicionales = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiariosProgramasProyectos", x => new { x.BeneficiarioID, x.ProgramaProyectoID });
                    table.ForeignKey(
                        name: "FK_BeneficiariosProgramasProyectos_Beneficiarios_BeneficiarioID",
                        column: x => x.BeneficiarioID,
                        principalTable: "Beneficiarios",
                        principalColumn: "BeneficiarioID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficiariosProgramasProyectos_ProgramasProyectosONG_ProgramaProyectoID",
                        column: x => x.ProgramaProyectoID,
                        principalTable: "ProgramasProyectosONG",
                        principalColumn: "ProgramaProyectoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeneficiarioGrupos",
                columns: table => new
                {
                    BeneficiarioID = table.Column<int>(type: "int", nullable: false),
                    GrupoID = table.Column<int>(type: "int", nullable: false),
                    FechaUnionGrupo = table.Column<DateTime>(type: "DATE", nullable: true),
                    RolEnGrupo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiarioGrupos", x => new { x.BeneficiarioID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_BeneficiarioGrupos_Beneficiarios_BeneficiarioID",
                        column: x => x.BeneficiarioID,
                        principalTable: "Beneficiarios",
                        principalColumn: "BeneficiarioID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficiarioGrupos_GruposComunitarios_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GruposComunitarios",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramaProyectoGrupos",
                columns: table => new
                {
                    ProgramaProyectoID = table.Column<int>(type: "int", nullable: false),
                    GrupoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramaProyectoGrupos", x => new { x.ProgramaProyectoID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_ProgramaProyectoGrupos_GruposComunitarios_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GruposComunitarios",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramaProyectoGrupos_ProgramasProyectosONG_ProgramaProyectoID",
                        column: x => x.ProgramaProyectoID,
                        principalTable: "ProgramasProyectosONG",
                        principalColumn: "ProgramaProyectoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipacionesActivas",
                columns: table => new
                {
                    ParticipacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudID = table.Column<int>(type: "int", nullable: false),
                    ProgramaProyectoID = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "DATE", nullable: false),
                    FechaInicioParticipacion = table.Column<DateTime>(type: "DATE", nullable: false),
                    FechaFinParticipacion = table.Column<DateTime>(type: "DATE", nullable: true),
                    RolDesempenado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    HorasTCUCompletadas = table.Column<int>(type: "int", nullable: true),
                    NotasSupervisor = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionesActivas", x => x.ParticipacionID);
                    table.ForeignKey(
                        name: "FK_ParticipacionesActivas_ProgramasProyectosONG_ProgramaProyectoID",
                        column: x => x.ProgramaProyectoID,
                        principalTable: "ProgramasProyectosONG",
                        principalColumn: "ProgramaProyectoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipacionesActivas_Solicitudes_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitudes",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudCamposInteres",
                columns: table => new
                {
                    SolicitudID = table.Column<int>(type: "int", nullable: false),
                    CampoInteresID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudCamposInteres", x => new { x.SolicitudID, x.CampoInteresID });
                    table.ForeignKey(
                        name: "FK_SolicitudCamposInteres_CamposInteresVocacional_CampoInteresID",
                        column: x => x.CampoInteresID,
                        principalTable: "CamposInteresVocacional",
                        principalColumn: "CampoInteresID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudCamposInteres_Solicitudes_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitudes",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesPrograma",
                columns: table => new
                {
                    EvaluacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipacionID = table.Column<int>(type: "int", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreProgramaUniversidadEvaluador = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParteMasGratificante = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ParteMasDificil = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    RazonesOriginalesParticipacion = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ExpectativasOriginalesCumplidas = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InformacionPreviaUtil = table.Column<int>(type: "int", nullable: true),
                    EsfuerzoIntegracionComunidades = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ComentariosAlojamientoHotel = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ProgramaInmersionCulturalAyudoHumildad = table.Column<int>(type: "int", nullable: true),
                    ActividadesRecreativasCulturalesInteresantes = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VisitaSitioComunidadFavoritaYPorQue = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AspectoMasValiosoExperiencia = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AplicaraLoAprendidoFuturo = table.Column<int>(type: "int", nullable: true),
                    TresCosasAprendidasSobreSiMismo = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    ComoCompartiraAprendidoUniversidad = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    RecomendariaProgramaOtros = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    QueDiraOtrosSobrePrograma = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    PermiteSerUsadoComoReferencia = table.Column<bool>(type: "bit", nullable: true),
                    ComentariosAdicionalesEvaluacion = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesPrograma", x => x.EvaluacionID);
                    table.ForeignKey(
                        name: "FK_EvaluacionesPrograma_ParticipacionesActivas_ParticipacionID",
                        column: x => x.ParticipacionID,
                        principalTable: "ParticipacionesActivas",
                        principalColumn: "ParticipacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiarioAsistenciaRecibida_TipoAsistenciaID",
                table: "BeneficiarioAsistenciaRecibida",
                column: "TipoAsistenciaID");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiarioGrupos_GrupoID",
                table: "BeneficiarioGrupos",
                column: "GrupoID");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiarios_ComunidadID",
                table: "Beneficiarios",
                column: "ComunidadID");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiariosProgramasProyectos_ProgramaProyectoID",
                table: "BeneficiariosProgramasProyectos",
                column: "ProgramaProyectoID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPrograma_ParticipacionID",
                table: "EvaluacionesPrograma",
                column: "ParticipacionID");

            migrationBuilder.CreateIndex(
                name: "IX_GruposComunitarios_ComunidadID",
                table: "GruposComunitarios",
                column: "ComunidadID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesActivas_ProgramaProyectoID",
                table: "ParticipacionesActivas",
                column: "ProgramaProyectoID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesActivas_SolicitudID",
                table: "ParticipacionesActivas",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaProyectoComunidades_ComunidadID",
                table: "ProgramaProyectoComunidades",
                column: "ComunidadID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramaProyectoGrupos_GrupoID",
                table: "ProgramaProyectoGrupos",
                column: "GrupoID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasProyectosONG_ResponsablePrincipalONGUsuarioID",
                table: "ProgramasProyectosONG",
                column: "ResponsablePrincipalONGUsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_PermisoID",
                table: "RolPermisos",
                column: "PermisoID");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudCamposInteres_CampoInteresID",
                table: "SolicitudCamposInteres",
                column: "CampoInteresID");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_FuenteConocimientoID",
                table: "Solicitudes",
                column: "FuenteConocimientoID");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_NivelIdiomaEspañolID",
                table: "Solicitudes",
                column: "NivelIdiomaEspañolID");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInformacionGeneral_FuenteConocimientoID",
                table: "SolicitudesInformacionGeneral",
                column: "FuenteConocimientoID");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInformacionGeneral_UsuarioAsignadoID",
                table: "SolicitudesInformacionGeneral",
                column: "UsuarioAsignadoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BeneficiarioAsistenciaRecibida");

            migrationBuilder.DropTable(
                name: "BeneficiarioGrupos");

            migrationBuilder.DropTable(
                name: "BeneficiariosProgramasProyectos");

            migrationBuilder.DropTable(
                name: "EvaluacionesPrograma");

            migrationBuilder.DropTable(
                name: "ProgramaProyectoComunidades");

            migrationBuilder.DropTable(
                name: "ProgramaProyectoGrupos");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "SolicitudCamposInteres");

            migrationBuilder.DropTable(
                name: "SolicitudesInformacionGeneral");

            migrationBuilder.DropTable(
                name: "TiposAsistencia");

            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "ParticipacionesActivas");

            migrationBuilder.DropTable(
                name: "GruposComunitarios");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "CamposInteresVocacional");

            migrationBuilder.DropTable(
                name: "ProgramasProyectosONG");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Comunidades");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FuentesConocimiento");

            migrationBuilder.DropTable(
                name: "NivelesIdioma");
        }
    }
}
