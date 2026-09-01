using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DATA.Migrations
{
    /// <inheritdoc />
    public partial class RelacionAuditoriaMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuditId",
                table: "Movements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movements_AuditId",
                table: "Movements",
                column: "AuditId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Audit_AuditId",
                table: "Movements",
                column: "AuditId",
                principalTable: "Audit",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Audit_AuditId",
                table: "Movements");

            migrationBuilder.DropIndex(
                name: "IX_Movements_AuditId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "AuditId",
                table: "Movements");
        }
    }
}
