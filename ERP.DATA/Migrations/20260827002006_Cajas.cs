using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class Cajas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShift_PosTerminal_PosTerminalId",
                table: "PosShift");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosShift_PosShiftId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PosShift",
                table: "PosShift");

            migrationBuilder.DropColumn(
                name: "CashierId",
                table: "PosShift");

            migrationBuilder.DropColumn(
                name: "CashierName",
                table: "PosShift");

            migrationBuilder.RenameTable(
                name: "PosShift",
                newName: "PosShifts");

            migrationBuilder.RenameIndex(
                name: "IX_PosShift_PosTerminalId",
                table: "PosShifts",
                newName: "IX_PosShifts_PosTerminalId");

            migrationBuilder.AddColumn<int>(
                name: "CajeroId",
                table: "PosShifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuariosId",
                table: "PosShifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PosShifts",
                table: "PosShifts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PrimerNombre = table.Column<string>(type: "text", nullable: false),
                    SegundoNombre = table.Column<string>(type: "text", nullable: true),
                    PrimerAPellido = table.Column<string>(type: "text", nullable: false),
                    SegundoAPellido = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_UsuariosId",
                table: "PosShifts",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_PosTerminal_PosTerminalId",
                table: "PosShifts",
                column: "PosTerminalId",
                principalTable: "PosTerminal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_Usuarios_UsuariosId",
                table: "PosShifts",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosShifts_PosShiftId",
                table: "Sales",
                column: "PosShiftId",
                principalTable: "PosShifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_PosTerminal_PosTerminalId",
                table: "PosShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_Usuarios_UsuariosId",
                table: "PosShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_PosShifts_PosShiftId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PosShifts",
                table: "PosShifts");

            migrationBuilder.DropIndex(
                name: "IX_PosShifts_UsuariosId",
                table: "PosShifts");

            migrationBuilder.DropColumn(
                name: "CajeroId",
                table: "PosShifts");

            migrationBuilder.DropColumn(
                name: "UsuariosId",
                table: "PosShifts");

            migrationBuilder.RenameTable(
                name: "PosShifts",
                newName: "PosShift");

            migrationBuilder.RenameIndex(
                name: "IX_PosShifts_PosTerminalId",
                table: "PosShift",
                newName: "IX_PosShift_PosTerminalId");

            migrationBuilder.AddColumn<string>(
                name: "CashierId",
                table: "PosShift",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CashierName",
                table: "PosShift",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PosShift",
                table: "PosShift",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShift_PosTerminal_PosTerminalId",
                table: "PosShift",
                column: "PosTerminalId",
                principalTable: "PosTerminal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_PosShift_PosShiftId",
                table: "Sales",
                column: "PosShiftId",
                principalTable: "PosShift",
                principalColumn: "Id");
        }
    }
}
