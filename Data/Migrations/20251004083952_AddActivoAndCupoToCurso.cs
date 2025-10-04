using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace examen_parcial_programacion1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivoAndCupoToCurso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Cursos");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Cursos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CupoMaximo",
                table: "Cursos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "CupoMaximo",
                table: "Cursos");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Cursos",
                type: "TEXT",
                nullable: true);
        }
    }
}
