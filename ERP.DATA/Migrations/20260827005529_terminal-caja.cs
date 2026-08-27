using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class terminalcaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_PosTerminal_PosTerminalId",
                table: "PosShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminal_Store_StoreId",
                table: "PosTerminal");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminal_Warehouse_WarehouseId",
                table: "PosTerminal");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminal_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PosTerminal",
                table: "PosTerminal");

            migrationBuilder.RenameTable(
                name: "PosTerminal",
                newName: "PosTerminals");

            migrationBuilder.RenameIndex(
                name: "IX_PosTerminal_WarehouseId",
                table: "PosTerminals",
                newName: "IX_PosTerminals_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_PosTerminal_StoreId",
                table: "PosTerminals",
                newName: "IX_PosTerminals_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PosTerminals",
                table: "PosTerminals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_PosTerminals_PosTerminalId",
                table: "PosShifts",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Store_StoreId",
                table: "PosTerminals",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminals_Warehouse_WarehouseId",
                table: "PosTerminals",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId",
                principalTable: "PosTerminals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_PosTerminals_PosTerminalId",
                table: "PosShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Store_StoreId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_PosTerminals_Warehouse_WarehouseId",
                table: "PosTerminals");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminals_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PosTerminals",
                table: "PosTerminals");

            migrationBuilder.RenameTable(
                name: "PosTerminals",
                newName: "PosTerminal");

            migrationBuilder.RenameIndex(
                name: "IX_PosTerminals_WarehouseId",
                table: "PosTerminal",
                newName: "IX_PosTerminal_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_PosTerminals_StoreId",
                table: "PosTerminal",
                newName: "IX_PosTerminal_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PosTerminal",
                table: "PosTerminal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_PosTerminal_PosTerminalId",
                table: "PosShifts",
                column: "PosTerminalId",
                principalTable: "PosTerminal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminal_Store_StoreId",
                table: "PosTerminal",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosTerminal_Warehouse_WarehouseId",
                table: "PosTerminal",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminal_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId",
                principalTable: "PosTerminal",
                principalColumn: "Id");
        }
    }
}
