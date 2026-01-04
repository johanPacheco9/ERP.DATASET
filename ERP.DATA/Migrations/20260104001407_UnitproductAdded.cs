using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class UnitproductAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoVarianteId = table.Column<int>(type: "integer", nullable: false),
                    BodegaId = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_UnitProduct_Bodegas_BodegaId",
                        column: x => x.BodegaId,
                        principalTable: "Bodegas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProduct_ProductoVariantes_ProductoVarianteId",
                        column: x => x.ProductoVarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitProduct_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
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
                name: "IX_UnitProduct_BodegaId",
                table: "UnitProduct",
                column: "BodegaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProduct_ProductoId",
                table: "UnitProduct",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProduct_ProductoVarianteId",
                table: "UnitProduct",
                column: "ProductoVarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitProductMovements_ProductoUnidadId",
                table: "UnitProductMovements",
                column: "ProductoUnidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitProductMovements");

            migrationBuilder.DropTable(
                name: "UnitProduct");
        }
    }
}
