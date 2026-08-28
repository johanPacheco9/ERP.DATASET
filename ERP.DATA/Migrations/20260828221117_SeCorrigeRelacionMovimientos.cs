using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class SeCorrigeRelacionMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouse_WarehouseId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductMovements");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "UnitProductMovements");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "UnitProductMovements",
                newName: "UnidadProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductMovements_ProductoVarianteId",
                table: "UnitProductMovements",
                newName: "IX_UnitProductMovements_UnidadProductoId");

            migrationBuilder.AlterColumn<int>(
                name: "WarehouseId",
                table: "Movements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "DestinationWarehouseId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrigenWarehouseId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Movements_DestinationWarehouseId",
                table: "Movements",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_OrigenWarehouseId",
                table: "Movements",
                column: "OrigenWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouse_DestinationWarehouseId",
                table: "Movements",
                column: "DestinationWarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouse_OrigenWarehouseId",
                table: "Movements",
                column: "OrigenWarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouse_WarehouseId",
                table: "Movements",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_UnidadesProductos_UnidadProductoId",
                table: "UnitProductMovements",
                column: "UnidadProductoId",
                principalTable: "UnidadesProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouse_DestinationWarehouseId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouse_OrigenWarehouseId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouse_WarehouseId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_UnidadesProductos_UnidadProductoId",
                table: "UnitProductMovements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_DestinationWarehouseId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_OrigenWarehouseId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "DestinationWarehouseId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "OrigenWarehouseId",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "UnidadProductoId",
                table: "UnitProductMovements",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductMovements_UnidadProductoId",
                table: "UnitProductMovements",
                newName: "IX_UnitProductMovements_ProductoVarianteId");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "UnitProductMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "WarehouseId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouse_WarehouseId",
                table: "Movements",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductMovements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
