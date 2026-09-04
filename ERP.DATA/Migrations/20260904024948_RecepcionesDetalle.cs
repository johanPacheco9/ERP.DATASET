using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class RecepcionesDetalle : Migration
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DniType = table.Column<int>(type: "integer", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dv = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    LegalOrganizationType = table.Column<string>(type: "text", nullable: true),
                    TaxRegime = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.Id);
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PrimerNombre = table.Column<string>(type: "text", nullable: false),
                    SegundoNombre = table.Column<string>(type: "text", nullable: true),
                    PrimerAPellido = table.Column<string>(type: "text", nullable: false),
                    SegundoAPellido = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProveedorId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Impuestos = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeCompra_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
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
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    BaseStatus = table.Column<int>(type: "integer", nullable: false),
                    MarcaId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ProveedorId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductoBase_Marca_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "Marca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoBase_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id");
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
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
                name: "UsuarioStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioStores_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioStores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraObservaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EstadoAsociado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraObservaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenCompraObservaciones_OrdenesDeCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesDeCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ObservacionesGenerales = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityReviews_OrdenesDeCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesDeCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecepcionesDeCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GuiaRemisionProveedor = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecepcionesDeCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecepcionesDeCompra_OrdenesDeCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesDeCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoBaseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoBaseId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoBaseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoBaseCategories_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoBaseCategories_ProductoBase_ProductoBaseId",
                        column: x => x.ProductoBaseId,
                        principalTable: "ProductoBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoBaseId = table.Column<int>(type: "integer", nullable: false),
                    ProveedorId = table.Column<int>(type: "integer", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    CodigoProveedor = table.Column<string>(type: "text", nullable: true),
                    DiasEntrega = table.Column<int>(type: "integer", nullable: true),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoProveedor_ProductoBase_ProductoBaseId",
                        column: x => x.ProductoBaseId,
                        principalTable: "ProductoBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoProveedor_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
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
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_ProductoBase_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductoBase",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audit_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PosTerminals",
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosTerminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosTerminals_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PosTerminals_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
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
                        name: "FK_DetalleOrdenCompra_OrdenesDeCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesDeCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleOrdenCompra_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "WarehouseStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    StockReservado = table.Column<int>(type: "integer", nullable: false),
                    StockMinimo = table.Column<int>(type: "integer", nullable: false),
                    StockMaximo = table.Column<int>(type: "integer", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseStock_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseStock_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuditId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditCategory_Audit_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
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
                    ProductoBaseId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
                    Serial = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OriginalUnitStatus = table.Column<int>(type: "integer", nullable: false),
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_UnitProductAudits_ProductoBase_ProductoBaseId",
                        column: x => x.ProductoBaseId,
                        principalTable: "ProductoBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductAudits_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
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
                name: "PosShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PosTerminalId = table.Column<int>(type: "integer", nullable: false),
                    CajeroId = table.Column<int>(type: "integer", nullable: false),
                    UsuariosId = table.Column<int>(type: "integer", nullable: false),
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
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosShifts_PosTerminals_PosTerminalId",
                        column: x => x.PosTerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PosShifts_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesRecepcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecepcionCompraId = table.Column<int>(type: "integer", nullable: false),
                    DetalleOrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    CantidadEsperada = table.Column<decimal>(type: "numeric", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "numeric", nullable: false),
                    ObservacionItem = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesRecepcion_DetalleOrdenCompra_DetalleOrdenCompraId",
                        column: x => x.DetalleOrdenCompraId,
                        principalTable: "DetalleOrdenCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesRecepcion_RecepcionesDeCompra_RecepcionCompraId",
                        column: x => x.RecepcionCompraId,
                        principalTable: "RecepcionesDeCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityReviewDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QualityReviewId = table.Column<int>(type: "integer", nullable: false),
                    DetalleOrdenCompraId = table.Column<int>(type: "integer", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "numeric", nullable: false),
                    CantidadAprobada = table.Column<decimal>(type: "numeric", nullable: false),
                    CantidadRechazada = table.Column<decimal>(type: "numeric", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityReviewDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityReviewDetails_DetalleOrdenCompra_DetalleOrdenCompraId",
                        column: x => x.DetalleOrdenCompraId,
                        principalTable: "DetalleOrdenCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualityReviewDetails_QualityReviews_QualityReviewId",
                        column: x => x.QualityReviewId,
                        principalTable: "QualityReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosShiftMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PosShiftId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosShiftMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosShiftMovements_PosShifts_PosShiftId",
                        column: x => x.PosShiftId,
                        principalTable: "PosShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    PosTerminalId = table.Column<int>(type: "integer", nullable: true),
                    PosShiftId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    FactusInvoiceNumber = table.Column<string>(type: "text", nullable: true),
                    FactusStatus = table.Column<string>(type: "text", nullable: true),
                    FactusCufe = table.Column<string>(type: "text", nullable: true),
                    FactusQrUrl = table.Column<string>(type: "text", nullable: true),
                    FactusXmlUrl = table.Column<string>(type: "text", nullable: true),
                    FactusPdfUrl = table.Column<string>(type: "text", nullable: true),
                    FactusErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_PosShifts_PosShiftId",
                        column: x => x.PosShiftId,
                        principalTable: "PosShifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sales_PosTerminals_PosTerminalId",
                        column: x => x.PosTerminalId,
                        principalTable: "PosTerminals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sales_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    Lote = table.Column<string>(type: "text", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Motive = table.Column<string>(type: "text", nullable: true),
                    Observations = table.Column<string>(type: "text", nullable: true),
                    OrigenWarehouseId = table.Column<int>(type: "integer", nullable: false),
                    DestinationWarehouseId = table.Column<int>(type: "integer", nullable: true),
                    AuditId = table.Column<int>(type: "integer", nullable: true),
                    SaleId = table.Column<int>(type: "integer", nullable: true),
                    CompraId = table.Column<int>(type: "integer", nullable: true),
                    OrdenDeCompraId = table.Column<int>(type: "integer", nullable: true),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movements_Audit_AuditId",
                        column: x => x.AuditId,
                        principalTable: "Audit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_OrdenesDeCompra_OrdenDeCompraId",
                        column: x => x.OrdenDeCompraId,
                        principalTable: "OrdenesDeCompra",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_Warehouse_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movements_Warehouse_OrigenWarehouseId",
                        column: x => x.OrigenWarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movements_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleLineItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    UnidadProductoId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    MovementId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleLineItems_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleLineItems_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleLineItems_UnidadesProductos_UnidadProductoId",
                        column: x => x.UnidadProductoId,
                        principalTable: "UnidadesProductos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SalePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalePayments_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitProductMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BodegaOrigenId = table.Column<int>(type: "integer", nullable: false),
                    BodegaDestinoId = table.Column<int>(type: "integer", nullable: true),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    TipoMovimiento = table.Column<int>(type: "integer", nullable: false),
                    UnidadProductoId = table.Column<int>(type: "integer", nullable: false),
                    MovimientoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitProductMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitProductMovements_Movements_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "Movements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProductMovements_UnidadesProductos_UnidadProductoId",
                        column: x => x.UnidadProductoId,
                        principalTable: "UnidadesProductos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_ProductId",
                table: "Audit",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_WarehouseId",
                table: "Audit",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditCategory_AuditId",
                table: "AuditCategory",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditCategory_CategoryId",
                table: "AuditCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdentificationNumber",
                table: "Clients",
                column: "IdentificationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenCompra_OrdenCompraId",
                table: "DetalleOrdenCompra",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOrdenCompra_ProductoVarianteId",
                table: "DetalleOrdenCompra",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesRecepcion_DetalleOrdenCompraId",
                table: "DetallesRecepcion",
                column: "DetalleOrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesRecepcion_RecepcionCompraId",
                table: "DetallesRecepcion",
                column: "RecepcionCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_AuditId",
                table: "Movements",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_DestinationWarehouseId",
                table: "Movements",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_OrdenDeCompraId",
                table: "Movements",
                column: "OrdenDeCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_OrigenWarehouseId",
                table: "Movements",
                column: "OrigenWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_SaleId",
                table: "Movements",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_WarehouseId",
                table: "Movements",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraObservaciones_OrdenCompraId",
                table: "OrdenCompraObservaciones",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeCompra_ProveedorId",
                table: "OrdenesDeCompra",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_PosShiftMovements_PosShiftId",
                table: "PosShiftMovements",
                column: "PosShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_PosTerminalId",
                table: "PosShifts",
                column: "PosTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_UsuariosId",
                table: "PosShifts",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_StoreId",
                table: "PosTerminals",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PosTerminals_WarehouseId",
                table: "PosTerminals",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBase_CategoryId",
                table: "ProductoBase",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBase_MarcaId",
                table: "ProductoBase",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBase_ProveedorId",
                table: "ProductoBase",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBaseCategories_CategoryId",
                table: "ProductoBaseCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBaseCategories_ProductoBaseId",
                table: "ProductoBaseCategories",
                column: "ProductoBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedor_ProductoBaseId",
                table: "ProductoProveedor",
                column: "ProductoBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedor_ProveedorId",
                table: "ProductoProveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_ProductoBaseId",
                table: "ProductoVariantes",
                column: "ProductoBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityReviewDetails_DetalleOrdenCompraId",
                table: "QualityReviewDetails",
                column: "DetalleOrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityReviewDetails_QualityReviewId",
                table: "QualityReviewDetails",
                column: "QualityReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityReviews_OrdenCompraId",
                table: "QualityReviews",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_RecepcionesDeCompra_OrdenCompraId",
                table: "RecepcionesDeCompra",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLineItems_ProductoVarianteId",
                table: "SaleLineItems",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLineItems_SaleId",
                table: "SaleLineItems",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLineItems_UnidadProductoId",
                table: "SaleLineItems",
                column: "UnidadProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_SalePayments_SaleId",
                table: "SalePayments",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ClientId",
                table: "Sales",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosShiftId",
                table: "Sales",
                column: "PosShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PosTerminalId",
                table: "Sales",
                column: "PosTerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleNumber",
                table: "Sales",
                column: "SaleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_StoreId",
                table: "Sales",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_WarehouseId",
                table: "Sales",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesProductos_BodegaId",
                table: "UnidadesProductos",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesProductos_ProductoVarianteId",
                table: "UnidadesProductos",
                column: "ProductoVarianteId");

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
                name: "IX_UnitProductAudits_ProductoBaseId",
                table: "UnitProductAudits",
                column: "ProductoBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductAudits_ProductoVarianteId",
                table: "UnitProductAudits",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_MovimientoId",
                table: "UnitProductMovements",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_UnidadProductoId",
                table: "UnitProductMovements",
                column: "UnidadProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioStores_StoreId",
                table: "UsuarioStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioStores_UsuarioId",
                table: "UsuarioStores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_StoreId",
                table: "Warehouse",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_ProductoVarianteId",
                table: "WarehouseStock",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStock_WarehouseId",
                table: "WarehouseStock",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditCategory");

            migrationBuilder.DropTable(
                name: "DetallesRecepcion");

            migrationBuilder.DropTable(
                name: "OrdenCompraObservaciones");

            migrationBuilder.DropTable(
                name: "PosShiftMovements");

            migrationBuilder.DropTable(
                name: "ProductoBaseCategories");

            migrationBuilder.DropTable(
                name: "ProductoProveedor");

            migrationBuilder.DropTable(
                name: "QualityReviewDetails");

            migrationBuilder.DropTable(
                name: "SaleLineItems");

            migrationBuilder.DropTable(
                name: "SalePayments");

            migrationBuilder.DropTable(
                name: "UnitProductAudits");

            migrationBuilder.DropTable(
                name: "UnitProductMovements");

            migrationBuilder.DropTable(
                name: "UsuarioStores");

            migrationBuilder.DropTable(
                name: "WarehouseStock");

            migrationBuilder.DropTable(
                name: "RecepcionesDeCompra");

            migrationBuilder.DropTable(
                name: "DetalleOrdenCompra");

            migrationBuilder.DropTable(
                name: "QualityReviews");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "UnidadesProductos");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "OrdenesDeCompra");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "ProductoVariantes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "PosShifts");

            migrationBuilder.DropTable(
                name: "ProductoBase");

            migrationBuilder.DropTable(
                name: "PosTerminals");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Marca");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "Store");
        }
    }
}
