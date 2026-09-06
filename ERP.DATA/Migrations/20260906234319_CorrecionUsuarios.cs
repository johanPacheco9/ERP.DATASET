using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class CorrecionUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrdenesDeCompra",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "OrdenesDeCompra",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrdenesDeCompra",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrdenesDeCompra",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "OrdenesDeCompra",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrdenesDeCompra");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OrdenesDeCompra");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrdenesDeCompra");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrdenesDeCompra");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "OrdenesDeCompra");
        }
    }
}
