using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddFiscalFieldsClienteProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StandardCode",
                table: "ProductoBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitMeasureCode",
                table: "ProductoBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LegalOrganizationCode",
                table: "Clients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MunicipalityCode",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TributeCode",
                table: "Clients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandardCode",
                table: "ProductoBase");

            migrationBuilder.DropColumn(
                name: "UnitMeasureCode",
                table: "ProductoBase");

            migrationBuilder.DropColumn(
                name: "LegalOrganizationCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "MunicipalityCode",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TributeCode",
                table: "Clients");
        }
    }
}
