using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class Reestructuracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Products_ProductId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductVariantId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_Products_ProductoId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_UnitProduct_UnitProductId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_UnitProduct_ProductoUnidadId",
                table: "UnitProductMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductVariantId",
                table: "WarehouseStock");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_Products_ProductId",
                table: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "UnitProduct");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStock_ProductId",
                table: "WarehouseStock");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStock_ProductVariantId",
                table: "WarehouseStock");

            migrationBuilder.DropIndex(
                name: "IX_UnitProductAudits_UnitProductId",
                table: "UnitProductAudits");

            migrationBuilder.DropIndex(
                name: "IX_Movements_ProductId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "WarehouseStock");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "WarehouseStock");

            migrationBuilder.RenameColumn(
                name: "ProductoUnidadId",
                table: "UnitProductMovements",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductMovements_ProductoUnidadId",
                table: "UnitProductMovements",
                newName: "IX_UnitProductMovements_ProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "UnitProductAudits",
                newName: "LineaProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_ProductoVarianteId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_LineaProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "Movements",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_ProductVariantId",
                table: "Movements",
                newName: "IX_Movements_ProductoId");

            migrationBuilder.AddColumn<int>(
                name: "LineaProductoId",
                table: "WarehouseStock",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovimientoId",
                table: "UnitProductMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineaProductoId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LineaProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeICA = table.Column<decimal>(type: "numeric", nullable: false),
                    ImpuestoEspecifico = table.Column<decimal>(type: "numeric", nullable: false),
                    ArancelImportacion = table.Column<decimal>(type: "numeric", nullable: false),
                    ExentoIVA = table.Column<bool>(type: "boolean", nullable: false),
                    GravadoICA = table.Column<bool>(type: "boolean", nullable: false),
                    CodigoTributario = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    Volumen = table.Column<decimal>(type: "numeric", nullable: false),
                    Dimensiones = table.Column<string>(type: "text", nullable: true),
                    EsPerecedero = table.Column<bool>(type: "boolean", nullable: false),
                    UnidadMedida = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequiereSerial = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineaProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineaProductos_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineaProductos_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LineaProductoId = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    CodigoBarras = table.Column<string>(type: "text", nullable: true),
                    Serial = table.Column<string>(type: "text", nullable: true),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "numeric", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: true),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Atributos = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_LineaProductos_LineaProductoId",
                        column: x => x.LineaProductoId,
                        principalTable: "LineaProductos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_Warehouse_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_LineaProductoId",
                table: "WarehouseStock",
                column: "LineaProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_MovimientoId",
                table: "UnitProductMovements",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_LineaProductoId",
                table: "Movements",
                column: "LineaProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_LineaProductos_CategoryId",
                table: "LineaProductos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LineaProductos_SupplierId",
                table: "LineaProductos",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_BodegaId",
                table: "Productos",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_LineaProductoId",
                table: "Productos",
                column: "LineaProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_LineaProductos_ProductId",
                table: "Audit",
                column: "ProductId",
                principalTable: "LineaProductos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_LineaProductos_LineaProductoId",
                table: "Movements",
                column: "LineaProductoId",
                principalTable: "LineaProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_LineaProductos_LineaProductoId",
                table: "UnitProductAudits",
                column: "LineaProductoId",
                principalTable: "LineaProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_Productos_ProductoId",
                table: "UnitProductAudits",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_Movements_MovimientoId",
                table: "UnitProductMovements",
                column: "MovimientoId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_Productos_ProductoId",
                table: "UnitProductMovements",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_LineaProductos_LineaProductoId",
                table: "WarehouseStock",
                column: "LineaProductoId",
                principalTable: "LineaProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_LineaProductos_ProductId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_LineaProductos_LineaProductoId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Productos_ProductoId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_LineaProductos_LineaProductoId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_Productos_ProductoId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_Movements_MovimientoId",
                table: "UnitProductMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_Productos_ProductoId",
                table: "UnitProductMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_LineaProductos_LineaProductoId",
                table: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "LineaProductos");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStock_LineaProductoId",
                table: "WarehouseStock");

            migrationBuilder.DropIndex(
                name: "IX_UnitProductMovements_MovimientoId",
                table: "UnitProductMovements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_LineaProductoId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "LineaProductoId",
                table: "WarehouseStock");

            migrationBuilder.DropColumn(
                name: "MovimientoId",
                table: "UnitProductMovements");

            migrationBuilder.DropColumn(
                name: "LineaProductoId",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "UnitProductMovements",
                newName: "ProductoUnidadId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductMovements_ProductoId",
                table: "UnitProductMovements",
                newName: "IX_UnitProductMovements_ProductoUnidadId");

            migrationBuilder.RenameColumn(
                name: "LineaProductoId",
                table: "UnitProductAudits",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_LineaProductoId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_ProductoVarianteId");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "Movements",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_ProductoId",
                table: "Movements",
                newName: "IX_Movements_ProductVariantId");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "WarehouseStock",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "WarehouseStock",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ArancelImportacion = table.Column<decimal>(type: "numeric", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CodigoTributario = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Dimensiones = table.Column<string>(type: "text", nullable: true),
                    Es_Perecedero = table.Column<bool>(type: "boolean", nullable: false),
                    ExentoIVA = table.Column<bool>(type: "boolean", nullable: false),
                    GravadoICA = table.Column<bool>(type: "boolean", nullable: false),
                    Imagen_Url = table.Column<string>(type: "text", nullable: true),
                    ImpuestoEspecifico = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeICA = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "numeric", nullable: false),
                    SaleCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Unidad_Medida = table.Column<string>(type: "text", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Volumen = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductoVariantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    Atributos = table.Column<string>(type: "text", nullable: true),
                    CodigoVariante = table.Column<string>(type: "text", nullable: false),
                    Codigo_Barras = table.Column<string>(type: "text", nullable: true),
                    Costo_Unitario = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    Fecha_Vencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    Precio_Venta = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoVariantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_Products_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Serial = table.Column<string>(type: "text", nullable: false),
                    UnitProductStatus = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitProduct_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProduct_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_ProductId",
                table: "WarehouseStock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_ProductVariantId",
                table: "WarehouseStock",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_UnitProductId",
                table: "UnitProductAudits",
                column: "UnitProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductId",
                table: "Movements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_ProductoId",
                table: "ProductoVariantes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProduct_ProductId",
                table: "UnitProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProduct_ProductoVarianteId",
                table: "UnitProduct",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProduct_WarehouseId",
                table: "UnitProduct",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Products_ProductId",
                table: "Audit",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductVariantId",
                table: "Movements",
                column: "ProductVariantId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductAudits",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_Products_ProductoId",
                table: "UnitProductAudits",
                column: "ProductoId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_UnitProduct_UnitProductId",
                table: "UnitProductAudits",
                column: "UnitProductId",
                principalTable: "UnitProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_UnitProduct_ProductoUnidadId",
                table: "UnitProductMovements",
                column: "ProductoUnidadId",
                principalTable: "UnitProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductVariantId",
                table: "WarehouseStock",
                column: "ProductVariantId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_Products_ProductId",
                table: "WarehouseStock",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
