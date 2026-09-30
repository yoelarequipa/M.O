using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEntregaFechaEntregada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntregada",
                table: "Entregas",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaEntregada",
                table: "Entregas");
        }
    }
}
