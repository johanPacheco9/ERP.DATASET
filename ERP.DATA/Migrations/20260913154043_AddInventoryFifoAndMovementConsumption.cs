using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryFifoAndMovementConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements");

            migrationBuilder.AddColumn<decimal>(
                name: "CostoVentaTotal",
                table: "SaleLineItems",
                type: "numeric(15,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingQuantity",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MovementConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExitMovementId = table.Column<int>(type: "integer", nullable: false),
                    EntryMovementId = table.Column<int>(type: "integer", nullable: false),
                    QuantityConsumed = table.Column<int>(type: "integer", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovementConsumptions_Movements_EntryMovementId",
                        column: x => x.EntryMovementId,
                        principalTable: "Movements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovementConsumptions_Movements_ExitMovementId",
                        column: x => x.ExitMovementId,
                        principalTable: "Movements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovementConsumptions_EntryMovementId",
                table: "MovementConsumptions",
                column: "EntryMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementConsumptions_ExitMovementId",
                table: "MovementConsumptions",
                column: "ExitMovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements");

            migrationBuilder.DropTable(
                name: "MovementConsumptions");

            migrationBuilder.DropColumn(
                name: "CostoVentaTotal",
                table: "SaleLineItems");

            migrationBuilder.DropColumn(
                name: "RemainingQuantity",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoVarianteId",
                table: "Movements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_ProductoVariantes_ProductoVarianteId",
                table: "Movements",
                column: "ProductoVarianteId",
                principalTable: "ProductoVariantes",
                principalColumn: "Id");
        }
    }
}
