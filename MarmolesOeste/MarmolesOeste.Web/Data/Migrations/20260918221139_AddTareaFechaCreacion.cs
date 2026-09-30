using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTareaFechaCreacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Tareas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Las tareas ya cargadas no tienen un instante de creación real; se usa
            // su fecha programada como mejor aproximación disponible.
            migrationBuilder.Sql(@"UPDATE ""Tareas"" SET ""FechaCreacion"" = ""Fecha"";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Tareas");
        }
    }
}
