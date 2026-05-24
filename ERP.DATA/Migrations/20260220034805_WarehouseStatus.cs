using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsMainStore = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Nit = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Ubication = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Max_Capacity = table.Column<decimal>(type: "numeric", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    SaleCost = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeIVA = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeICA = table.Column<decimal>(type: "numeric", nullable: false),
                    ImpuestoEspecifico = table.Column<decimal>(type: "numeric", nullable: false),
                    ArancelImportacion = table.Column<decimal>(type: "numeric", nullable: false),
                    ExentoIVA = table.Column<bool>(type: "boolean", nullable: false),
                    GravadoICA = table.Column<bool>(type: "boolean", nullable: false),
                    CodigoTributario = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    Unidad_Medida = table.Column<string>(type: "text", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    Volumen = table.Column<decimal>(type: "numeric", nullable: false),
                    Dimensiones = table.Column<string>(type: "text", nullable: true),
                    Es_Perecedero = table.Column<bool>(type: "boolean", nullable: false),
                    Imagen_Url = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponsibleId = table.Column<int>(type: "integer", nullable: false),
                    SupervisorId = table.Column<int>(type: "integer", nullable: true),
                    TotalExpectedUnits = table.Column<int>(type: "integer", nullable: false),
                    TotalCountedUnits = table.Column<int>(type: "integer", nullable: false),
                    TotalMatches = table.Column<int>(type: "integer", nullable: false),
                    TotalMissing = table.Column<int>(type: "integer", nullable: false),
                    TotalSurplus = table.Column<int>(type: "integer", nullable: false),
                    TotalLocationDifferences = table.Column<int>(type: "integer", nullable: false),
                    TotalStatusDifferences = table.Column<int>(type: "integer", nullable: false),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    Conclusions = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audit_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audit_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductoVariantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    CodigoVariante = table.Column<string>(type: "text", nullable: false),
                    Codigo_Barras = table.Column<string>(type: "text", nullable: true),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    Fecha_Vencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Precio_Venta = table.Column<decimal>(type: "numeric", nullable: true),
                    Costo_Unitario = table.Column<decimal>(type: "numeric", nullable: true),
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
                    table.PrimaryKey("PK_ProductoVariantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoVariantes_Products_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    ReferenceId = table.Column<int>(type: "integer", nullable: true),
                    ReferenceTye = table.Column<string>(type: "text", nullable: true),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Motive = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movements_ProductoVariantes_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
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
                    Serial = table.Column<string>(type: "text", nullable: false),
                    UnitProductStatus = table.Column<int>(type: "integer", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WarehouseStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    StockMinimo = table.Column<int>(type: "integer", nullable: false),
                    StockMaximo = table.Column<int>(type: "integer", nullable: false),
                    StockReservado = table.Column<int>(type: "integer", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseStock_ProductoVariantes_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseStock_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseStock_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitProductAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditId = table.Column<int>(type: "integer", nullable: false),
                    UnitProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    Serial = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    MotivoDiferencia = table.Column<string>(type: "text", nullable: true),
                    BodegaEncontrada = table.Column<int>(type: "integer", nullable: true),
                    UbicacionFisica = table.Column<string>(type: "text", nullable: true),
                    EstadoFisico = table.Column<string>(type: "text", nullable: true),
                    RequiereAccionCorrectiva = table.Column<bool>(type: "boolean", nullable: false),
                    AjusteRealizado = table.Column<bool>(type: "boolean", nullable: false),
                    MovimientoAjusteId = table.Column<int>(type: "integer", nullable: true),
                    FechaAjuste = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BodegaEncontradaNavId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitProductAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_Audit_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_Products_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_UnitProduct_UnitProductId",
                        column: x => x.UnitProductId,
                        principalTable: "UnitProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_Warehouse_BodegaEncontradaNavId",
                        column: x => x.BodegaEncontradaNavId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_Warehouse_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitProductMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoUnidadId = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimiento = table.Column<int>(type: "integer", nullable: false),
                    BodegaOrigenId = table.Column<int>(type: "integer", nullable: false),
                    BodegaDestinoId = table.Column<int>(type: "integer", nullable: true),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitProductMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitProductMovements_UnitProduct_ProductoUnidadId",
                        column: x => x.ProductoUnidadId,
                        principalTable: "UnitProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CategoryId",
                table: "Audit",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_ProductId",
                table: "Audit",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_WarehouseId",
                table: "Audit",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductId",
                table: "Movements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductVariantId",
                table: "Movements",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_WarehouseId",
                table: "Movements",
                column: "WarehouseId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_AuditId",
                table: "UnitProductAudits",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_BodegaEncontradaNavId",
                table: "UnitProductAudits",
                column: "BodegaEncontradaNavId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_BodegaId",
                table: "UnitProductAudits",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_ProductoId",
                table: "UnitProductAudits",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_ProductoVarianteId",
                table: "UnitProductAudits",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_UnitProductId",
                table: "UnitProductAudits",
                column: "UnitProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_ProductoUnidadId",
                table: "UnitProductMovements",
                column: "ProductoUnidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_StoreId",
                table: "Warehouse",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_ProductId",
                table: "WarehouseStock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_ProductVariantId",
                table: "WarehouseStock",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_WarehouseId",
                table: "WarehouseStock",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "UnitProductAudits");

            migrationBuilder.DropTable(
                name: "UnitProductMovements");

            migrationBuilder.DropTable(
                name: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "UnitProduct");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
