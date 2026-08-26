using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class SalesTaxAndFactus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FactusCufe",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusErrorMessage",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusInvoiceNumber",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusPdfUrl",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusQrUrl",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusStatus",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactusXmlUrl",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "SaleLineItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "SaleLineItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Dv",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalOrganizationType",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxRegime",
                table: "Clients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactusCufe",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusErrorMessage",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusInvoiceNumber",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusPdfUrl",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusQrUrl",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusStatus",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FactusXmlUrl",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "SaleLineItems");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "SaleLineItems");

            migrationBuilder.DropColumn(
                name: "Dv",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LegalOrganizationType",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxRegime",
                table: "Clients");
        }
    }
}
