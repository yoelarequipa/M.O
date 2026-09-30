using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarmolesOeste.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColorYCalados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorMaterialCongelado",
                table: "PresupuestoPiezas",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#cfe3d8");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Materiales",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#cfe3d8");

            migrationBuilder.CreateTable(
                name: "PresupuestoPiezaCalados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PresupuestoPiezaId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    TramoOrden = table.Column<int>(type: "integer", nullable: false),
                    Ancho = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Fondo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DistanciaLateral = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DistanciaFrontal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresupuestoPiezaCalados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresupuestoPiezaCalados_PresupuestoPiezas_PresupuestoPiezaId",
                        column: x => x.PresupuestoPiezaId,
                        principalTable: "PresupuestoPiezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PresupuestoPiezaCalados_PresupuestoPiezaId",
                table: "PresupuestoPiezaCalados",
                column: "PresupuestoPiezaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresupuestoPiezaCalados");

            migrationBuilder.DropColumn(
                name: "ColorMaterialCongelado",
                table: "PresupuestoPiezas");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Materiales");
        }
    }
}
