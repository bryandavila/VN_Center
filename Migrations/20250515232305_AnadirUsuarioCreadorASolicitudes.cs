using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VN_Center.Migrations
{
    /// <inheritdoc />
    public partial class AnadirUsuarioCreadorASolicitudes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreadorId",
                table: "Solicitudes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioCreadorId",
                table: "Solicitudes");
        }
    }
}
