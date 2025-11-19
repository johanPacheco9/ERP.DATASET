using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class buildersupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_ProductoVariante_ProductoVarianteId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBodegas_ProductoVariante_ProductoVarianteId",
                table: "StockBodegas");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoVarianteId",
                table: "StockBodegas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductoId",
                table: "StockBodegas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoVarianteId",
                table: "Movimientos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductoId",
                table: "Movimientos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockBodegas_ProductoId",
                table: "StockBodegas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ProductoId",
                table: "Movimientos",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_ProductoVariante_ProductoVarianteId",
                table: "Movimientos",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariante",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockBodegas_ProductoVariante_ProductoVarianteId",
                table: "StockBodegas",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariante",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockBodegas_Productos_ProductoId",
                table: "StockBodegas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_ProductoVariante_ProductoVarianteId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBodegas_ProductoVariante_ProductoVarianteId",
                table: "StockBodegas");

            migrationBuilder.DropForeignKey(
                name: "FK_StockBodegas_Productos_ProductoId",
                table: "StockBodegas");

            migrationBuilder.DropIndex(
                name: "IX_StockBodegas_ProductoId",
                table: "StockBodegas");

            migrationBuilder.DropIndex(
                name: "IX_Movimientos_ProductoId",
                table: "Movimientos");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "StockBodegas");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Movimientos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoVarianteId",
                table: "StockBodegas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductoVarianteId",
                table: "Movimientos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_ProductoVariante_ProductoVarianteId",
                table: "Movimientos",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockBodegas_ProductoVariante_ProductoVarianteId",
                table: "StockBodegas",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
