using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTareaVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VentaId",
                table: "Tareas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_VentaId",
                table: "Tareas",
                column: "VentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Ventas_VentaId",
                table: "Tareas",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Ventas_VentaId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_VentaId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "VentaId",
                table: "Tareas");
        }
    }
}
