using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePosShifNavigationCajero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_Usuarios_UsuariosId",
                table: "PosShifts");

            migrationBuilder.DropIndex(
                name: "IX_PosShifts_UsuariosId",
                table: "PosShifts");

            migrationBuilder.DropColumn(
                name: "UsuariosId",
                table: "PosShifts");

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_CajeroId",
                table: "PosShifts",
                column: "CajeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_Usuarios_CajeroId",
                table: "PosShifts",
                column: "CajeroId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosShifts_Usuarios_CajeroId",
                table: "PosShifts");

            migrationBuilder.DropIndex(
                name: "IX_PosShifts_CajeroId",
                table: "PosShifts");

            migrationBuilder.AddColumn<int>(
                name: "UsuariosId",
                table: "PosShifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_UsuariosId",
                table: "PosShifts",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_PosShifts_Usuarios_UsuariosId",
                table: "PosShifts",
                column: "UsuariosId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
