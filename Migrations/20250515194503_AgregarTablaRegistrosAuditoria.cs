using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VN_Center.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaRegistrosAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosAuditoria",
                columns: table => new
                {
                    AuditoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHoraEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEjecutorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    NombreUsuarioEjecutor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TipoEvento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntidadAfectada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdEntidadAfectada = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    DetallesCambio = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    DireccionIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAuditoria", x => x.AuditoriaID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosAuditoria");
        }
    }
}
