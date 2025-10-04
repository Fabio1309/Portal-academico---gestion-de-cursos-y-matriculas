using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace examen_parcial_programacion1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoAndFechaToMatricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaMatricula",
                table: "Matriculas",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "Activa",
                table: "Matriculas",
                newName: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "Matriculas",
                newName: "FechaMatricula");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Matriculas",
                newName: "Activa");
        }
    }
}
