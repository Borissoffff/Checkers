using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DB.Migrations
{
    /// <inheritdoc />
    public partial class EditMovementLog2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovementLogs_DbCoordinates_MovementFromId",
                table: "MovementLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementLogs_DbCoordinates_MovementToId",
                table: "MovementLogs");

            migrationBuilder.DropIndex(
                name: "IX_MovementLogs_MovementFromId",
                table: "MovementLogs");

            migrationBuilder.DropIndex(
                name: "IX_MovementLogs_MovementToId",
                table: "MovementLogs");

            migrationBuilder.RenameColumn(
                name: "MovementToId",
                table: "MovementLogs",
                newName: "MovementToY");

            migrationBuilder.RenameColumn(
                name: "MovementFromId",
                table: "MovementLogs",
                newName: "MovementToX");

            migrationBuilder.AddColumn<int>(
                name: "EatenCheckerX",
                table: "MovementLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EatenCheckerY",
                table: "MovementLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovementFromX",
                table: "MovementLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovementFromY",
                table: "MovementLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EatenCheckerX",
                table: "MovementLogs");

            migrationBuilder.DropColumn(
                name: "EatenCheckerY",
                table: "MovementLogs");

            migrationBuilder.DropColumn(
                name: "MovementFromX",
                table: "MovementLogs");

            migrationBuilder.DropColumn(
                name: "MovementFromY",
                table: "MovementLogs");

            migrationBuilder.RenameColumn(
                name: "MovementToY",
                table: "MovementLogs",
                newName: "MovementToId");

            migrationBuilder.RenameColumn(
                name: "MovementToX",
                table: "MovementLogs",
                newName: "MovementFromId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_MovementFromId",
                table: "MovementLogs",
                column: "MovementFromId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_MovementToId",
                table: "MovementLogs",
                column: "MovementToId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovementLogs_DbCoordinates_MovementFromId",
                table: "MovementLogs",
                column: "MovementFromId",
                principalTable: "DbCoordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementLogs_DbCoordinates_MovementToId",
                table: "MovementLogs",
                column: "MovementToId",
                principalTable: "DbCoordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
