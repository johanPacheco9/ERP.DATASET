using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class OrdenDeCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "Movements",
                newName: "SaleId");

            migrationBuilder.AddColumn<int>(
                name: "CompraId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrdenDeCompraId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrdenCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProveedorId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Impuestos = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Supplier_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleOrdenCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Impuesto = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleOrdenCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenCompra_OrdenCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenCompra_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movements_OrdenDeCompraId",
                table: "Movements",
                column: "OrdenDeCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_SaleId",
                table: "Movements",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenCompra_OrdenCompraId",
                table: "DetalleOrdenCompra",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenCompra_ProductoVarianteId",
                table: "DetalleOrdenCompra",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_ProveedorId",
                table: "OrdenCompra",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_OrdenCompra_OrdenDeCompraId",
                table: "Movements",
                column: "OrdenDeCompraId",
                principalTable: "OrdenCompra",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Sales_SaleId",
                table: "Movements",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_OrdenCompra_OrdenDeCompraId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Sales_SaleId",
                table: "Movements");

            migrationBuilder.DropTable(
                name: "DetalleOrdenCompra");

            migrationBuilder.DropTable(
                name: "OrdenCompra");

            migrationBuilder.DropIndex(
                name: "IX_Movements_OrdenDeCompraId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_SaleId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "CompraId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "OrdenDeCompraId",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "SaleId",
                table: "Movements",
                newName: "ReferenceId");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "Movements",
                type: "text",
                nullable: true);
        }
    }
}
