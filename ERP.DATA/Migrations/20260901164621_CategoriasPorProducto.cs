using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class CategoriasPorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Category_CategoryId",
                table: "Audit");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoBase_Category_CategoryId",
                table: "ProductoBase");

            migrationBuilder.DropIndex(
                name: "IX_Audit_CategoryId",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Audit");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ProductoBase",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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

            migrationBuilder.CreateIndex(
                name: "IX_AuditCategory_AuditId",
                table: "AuditCategory",
                column: "AuditId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditCategory_CategoryId",
                table: "AuditCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBaseCategories_CategoryId",
                table: "ProductoBaseCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoBaseCategories_ProductoBaseId",
                table: "ProductoBaseCategories",
                column: "ProductoBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoBase_Category_CategoryId",
                table: "ProductoBase",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoBase_Category_CategoryId",
                table: "ProductoBase");

            migrationBuilder.DropTable(
                name: "AuditCategory");

            migrationBuilder.DropTable(
                name: "ProductoBaseCategories");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ProductoBase",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Audit",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audit_CategoryId",
                table: "Audit",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Category_CategoryId",
                table: "Audit",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoBase_Category_CategoryId",
                table: "ProductoBase",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
