using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaActualizacionCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "TerminacionesCanto",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoUltimaActualizacion",
                table: "TerminacionesCanto",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Materiales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoUltimaActualizacion",
                table: "Materiales",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Adicionales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoUltimaActualizacion",
                table: "Adicionales",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "TerminacionesCanto");

            migrationBuilder.DropColumn(
                name: "MotivoUltimaActualizacion",
                table: "TerminacionesCanto");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "MotivoUltimaActualizacion",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "Adicionales");

            migrationBuilder.DropColumn(
                name: "MotivoUltimaActualizacion",
                table: "Adicionales");
        }
    }
}
