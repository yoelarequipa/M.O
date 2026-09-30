using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertaStockMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlertaStockActiva",
                table: "Materiales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AlertaStockCantidad",
                table: "Materiales",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertaStockActiva",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "AlertaStockCantidad",
                table: "Materiales");
        }
    }
}
