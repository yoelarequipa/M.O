using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoTarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Tareas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(@"UPDATE ""Tareas"" SET ""Estado"" = 3 WHERE ""Completada"" = true;");

            migrationBuilder.DropColumn(
                name: "Completada",
                table: "Tareas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completada",
                table: "Tareas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"UPDATE ""Tareas"" SET ""Completada"" = true WHERE ""Estado"" = 3;");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Tareas");
        }
    }
}
