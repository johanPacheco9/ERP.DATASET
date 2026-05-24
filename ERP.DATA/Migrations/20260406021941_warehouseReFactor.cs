using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class warehouseReFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductoVarianteId",
                table: "WarehouseStock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoVarianteId",
                table: "WarehouseStock",
                type: "integer",
                nullable: true);
        }
    }
}
