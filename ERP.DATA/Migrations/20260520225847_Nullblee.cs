using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class Nullblee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Movements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
