using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VN_Center.Migrations
{
    /// <inheritdoc />
    public partial class AnadirUsuarioEvaluadorAEvaluacionesPrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioEvaluadorId",
                table: "EvaluacionesPrograma",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioEvaluadorId",
                table: "EvaluacionesPrograma");
        }
    }
}
