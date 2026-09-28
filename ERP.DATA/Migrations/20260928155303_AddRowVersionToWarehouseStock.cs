using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToWarehouseStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_Warehouse_WarehouseId",
                table: "WarehouseStock");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStock_WarehouseId",
                table: "WarehouseStock");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "WarehouseStock");

            migrationBuilder.AlterColumn<int>(
                name: "StockReservado",
                table: "WarehouseStock",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "WarehouseStock",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_WarehouseVariant_Unique",
                table: "WarehouseStock",
                columns: new[] { "WarehouseId", "ProductoVarianteId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_Warehouse_WarehouseId",
                table: "WarehouseStock",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_Warehouse_WarehouseId",
                table: "WarehouseStock");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStock_WarehouseVariant_Unique",
                table: "WarehouseStock");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "WarehouseStock");

            migrationBuilder.AlterColumn<int>(
                name: "StockReservado",
                table: "WarehouseStock",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "WarehouseStock",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_WarehouseId",
                table: "WarehouseStock",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_Warehouse_WarehouseId",
                table: "WarehouseStock",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
