using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class Movimientoupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_ProductoVariantes_ProductoVarianteId",
                table: "Movimientos");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductoVariantes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movimientos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_ProductoVariantes_ProductoVarianteId",
                table: "Movimientos",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_ProductoVariantes_ProductoVarianteId",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductoVariantes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movimientos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_ProductoVariantes_ProductoVarianteId",
                table: "Movimientos",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id");
        }
    }
}
