using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VN_Center.Migrations
{
    /// <inheritdoc />
    public partial class AnadirUsuarioCreadorAGruposComunitarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreadorId",
                table: "GruposComunitarios",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioCreadorId",
                table: "GruposComunitarios");
        }
    }
}
