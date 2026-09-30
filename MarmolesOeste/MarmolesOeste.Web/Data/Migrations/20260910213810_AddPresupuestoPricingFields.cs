using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPresupuestoPricingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DescuentoPorcentaje",
                table: "Presupuestos",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IvaPorcentaje",
                table: "Presupuestos",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioZocaloCongelado",
                table: "PresupuestoPiezas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescuentoPorcentaje",
                table: "Presupuestos");

            migrationBuilder.DropColumn(
                name: "IvaPorcentaje",
                table: "Presupuestos");

            migrationBuilder.DropColumn(
                name: "PrecioZocaloCongelado",
                table: "PresupuestoPiezas");
        }
    }
}
