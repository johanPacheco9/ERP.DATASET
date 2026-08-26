using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AjusteProductosNombres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_SaleLineItems_LineaProductos_LineaProductoId",
                table: "SaleLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleLineItems_Productos_ProductoId",
                table: "SaleLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_LineaProductos_LineaProductoId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_Productos_ProductoId",
                table: "UnitProductAudits");

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
                name: "IX_UnitProductMovements_ProductoId",
                table: "UnitProductMovements");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Movements");

            migrationBuilder.RenameColumn(
                name: "LineaProductoId",
                table: "WarehouseStock",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseStock_LineaProductoId",
                table: "WarehouseStock",
                newName: "IX_WarehouseStock_ProductoVarianteId");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "UnitProductAudits",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameColumn(
                name: "LineaProductoId",
                table: "UnitProductAudits",
                newName: "ProductoBaseId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_ProductoId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_LineaProductoId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_ProductoBaseId");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "SaleLineItems",
                newName: "UnidadProductoId");

            migrationBuilder.RenameColumn(
                name: "LineaProductoId",
                table: "SaleLineItems",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleLineItems_ProductoId",
                table: "SaleLineItems",
                newName: "IX_SaleLineItems_UnidadProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleLineItems_LineaProductoId",
                table: "SaleLineItems",
                newName: "IX_SaleLineItems_ProductoVarianteId");

            migrationBuilder.RenameColumn(
                name: "ReferenceTye",
                table: "Movements",
                newName: "ReferenceType");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "Movements",
                newName: "UnidadProductoId");

            migrationBuilder.RenameColumn(
                name: "LineaProductoId",
                table: "Movements",
                newName: "ProductoVarianteId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_ProductoId",
                table: "Movements",
                newName: "IX_Movements_UnidadProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_LineaProductoId",
                table: "Movements",
                newName: "IX_Movements_ProductoVarianteId");

            migrationBuilder.AddColumn<int>(
                name: "ProductoVarianteId",
                table: "UnitProductMovements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PosShiftId",
                table: "Sales",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PosTerminalId",
                table: "Sales",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "Movements",
                type: "numeric(15,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateTable(
                name: "PosTerminal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    Prefix = table.Column<string>(type: "text", nullable: false),
                    CurrentConsecutive = table.Column<long>(type: "bigint", nullable: false),
                    DianResolutionNumber = table.Column<string>(type: "text", nullable: true),
                    DianResolutionDate = table.Column<string>(type: "text", nullable: true),
                    FromNumber = table.Column<long>(type: "bigint", nullable: false),
                    ToNumber = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTerminal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosTerminal_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PosTerminal_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoBase",
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
                    RequiereSerial = table.Column<bool>(type: "boolean", nullable: false),
                    UnidadMedida = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    BaseStatus = table.Column<int>(type: "integer", nullable: false),
                    ProveedorId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoBase_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoBase_Supplier_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PosShift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PosTerminalId = table.Column<int>(type: "integer", nullable: false),
                    CashierId = table.Column<string>(type: "text", nullable: false),
                    CashierName = table.Column<string>(type: "text", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InitialCash = table.Column<decimal>(type: "numeric", nullable: false),
                    CashSales = table.Column<decimal>(type: "numeric", nullable: false),
                    CardSales = table.Column<decimal>(type: "numeric", nullable: false),
                    TransferSales = table.Column<decimal>(type: "numeric", nullable: false),
                    CreditSales = table.Column<decimal>(type: "numeric", nullable: false),
                    CashWithdrawals = table.Column<decimal>(type: "numeric", nullable: false),
                    CashAdditions = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalExpectedCash = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualCash = table.Column<decimal>(type: "numeric", nullable: true),
                    Difference = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosShift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosShift_PosTerminal_PosTerminalId",
                        column: x => x.PosTerminalId,
                        principalTable: "PosTerminal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoVariantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoBaseId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    CodigoBarras = table.Column<string>(type: "text", nullable: true),
                    Atributos = table.Column<string>(type: "text", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "numeric", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoVariantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_ProductoBase_ProductoBaseId",
                        column: x => x.ProductoBaseId,
                        principalTable: "ProductoBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    SerialNumber = table.Column<string>(type: "text", nullable: false),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UbicacionFisica = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnidadesProductos_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnidadesProductos_Warehouse_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_ProductoVarianteId",
                table: "UnitProductMovements",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosShiftId",
                table: "Sales",
                column: "PosShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_PosShift_PosTerminalId",
                table: "PosShift",
                column: "PosTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminal_StoreId",
                table: "PosTerminal",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminal_WarehouseId",
                table: "PosTerminal",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBase_CategoryId",
                table: "ProductoBase",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBase_ProveedorId",
                table: "ProductoBase",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_ProductoBaseId",
                table: "ProductoVariantes",
                column: "ProductoBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesProductos_BodegaId",
                table: "UnidadesProductos",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesProductos_ProductoVarianteId",
                table: "UnidadesProductos",
                column: "ProductoVarianteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_ProductoBase_ProductId",
                table: "Audit",
                column: "ProductId",
                principalTable: "ProductoBase",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_UnidadesProductos_UnidadProductoId",
                table: "Movements",
                column: "UnidadProductoId",
                principalTable: "UnidadesProductos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLineItems_ProductoVariantes_ProductoVarianteId",
                table: "SaleLineItems",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLineItems_UnidadesProductos_UnidadProductoId",
                table: "SaleLineItems",
                column: "UnidadProductoId",
                principalTable: "UnidadesProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosShift_PosShiftId",
                table: "Sales",
                column: "PosShiftId",
                principalTable: "PosShift",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosTerminal_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId",
                principalTable: "PosTerminal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_ProductoBase_ProductoBaseId",
                table: "UnitProductAudits",
                column: "ProductoBaseId",
                principalTable: "ProductoBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductAudits",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitProductMovements_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductMovements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_ProductoBase_ProductId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_UnidadesProductos_UnidadProductoId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleLineItems_ProductoVariantes_ProductoVarianteId",
                table: "SaleLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleLineItems_UnidadesProductos_UnidadProductoId",
                table: "SaleLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosShift_PosShiftId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosTerminal_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_ProductoBase_ProductoBaseId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitProductMovements_ProductoVariantes_ProductoVarianteId",
                table: "UnitProductMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                table: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "PosShift");

            migrationBuilder.DropTable(
                name: "UnidadesProductos");

            migrationBuilder.DropTable(
                name: "PosTerminal");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "ProductoBase");

            migrationBuilder.DropIndex(
                name: "IX_UnitProductMovements_ProductoVarianteId",
                table: "UnitProductMovements");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PosShiftId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ProductoVarianteId",
                table: "UnitProductMovements");

            migrationBuilder.DropColumn(
                name: "PosShiftId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PosTerminalId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "WarehouseStock",
                newName: "LineaProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_WarehouseStock_ProductoVarianteId",
                table: "WarehouseStock",
                newName: "IX_WarehouseStock_LineaProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "UnitProductAudits",
                newName: "ProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductoBaseId",
                table: "UnitProductAudits",
                newName: "LineaProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_ProductoVarianteId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_UnitProductAudits_ProductoBaseId",
                table: "UnitProductAudits",
                newName: "IX_UnitProductAudits_LineaProductoId");

            migrationBuilder.RenameColumn(
                name: "UnidadProductoId",
                table: "SaleLineItems",
                newName: "ProductoId");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "SaleLineItems",
                newName: "LineaProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleLineItems_UnidadProductoId",
                table: "SaleLineItems",
                newName: "IX_SaleLineItems_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleLineItems_ProductoVarianteId",
                table: "SaleLineItems",
                newName: "IX_SaleLineItems_LineaProductoId");

            migrationBuilder.RenameColumn(
                name: "UnidadProductoId",
                table: "Movements",
                newName: "ProductoId");

            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                table: "Movements",
                newName: "ReferenceTye");

            migrationBuilder.RenameColumn(
                name: "ProductoVarianteId",
                table: "Movements",
                newName: "LineaProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_UnidadProductoId",
                table: "Movements",
                newName: "IX_Movements_ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_ProductoVarianteId",
                table: "Movements",
                newName: "IX_Movements_LineaProductoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "Movements",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(15,4)");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LineaProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ArancelImportacion = table.Column<decimal>(type: "numeric", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CodigoTributario = table.Column<string>(type: "text", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Dimensiones = table.Column<string>(type: "text", nullable: true),
                    EsPerecedero = table.Column<bool>(type: "boolean", nullable: false),
                    ExentoIVA = table.Column<bool>(type: "boolean", nullable: false),
                    GravadoICA = table.Column<bool>(type: "boolean", nullable: false),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    ImpuestoEspecifico = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeICA = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "numeric", nullable: false),
                    RequiereSerial = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    UnidadMedida = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Volumen = table.Column<decimal>(type: "numeric", nullable: false)
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
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    LineaProductoId = table.Column<int>(type: "integer", nullable: false),
                    Atributos = table.Column<string>(type: "text", nullable: true),
                    CodigoBarras = table.Column<string>(type: "text", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    PrecioVenta = table.Column<decimal>(type: "numeric", nullable: true),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    Serial = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
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
                name: "IX_UnitProductMovements_ProductoId",
                table: "UnitProductMovements",
                column: "ProductoId");

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
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLineItems_LineaProductos_LineaProductoId",
                table: "SaleLineItems",
                column: "LineaProductoId",
                principalTable: "LineaProductos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLineItems_Productos_ProductoId",
                table: "SaleLineItems",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
    }
}
