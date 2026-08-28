using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class NormalizacionMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_UnidadesProductos_UnidadProductoId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_UnidadProductoId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "UnidadProductoId",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnidadProductoId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movements_UnidadProductoId",
                table: "Movements",
                column: "UnidadProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_UnidadesProductos_UnidadProductoId",
                table: "Movements",
                column: "UnidadProductoId",
                principalTable: "UnidadesProductos",
                principalColumn: "Id");
        }
    }
}
