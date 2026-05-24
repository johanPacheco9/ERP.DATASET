using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class MovementReFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductoVarianteId",
                table: "Movements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoVarianteId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
