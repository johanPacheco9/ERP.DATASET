using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class RelacionCajeroTiendaActualizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_StoreId",
                table: "Usuarios",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Store_StoreId",
                table: "Usuarios",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Store_StoreId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_StoreId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Usuarios");
        }
    }
}
