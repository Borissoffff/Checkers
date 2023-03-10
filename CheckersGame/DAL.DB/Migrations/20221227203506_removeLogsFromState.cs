using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DB.Migrations
{
    /// <inheritdoc />
    public partial class removeLogsFromState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovementLogs_CheckersGameStates_CheckersGameStateId",
                table: "MovementLogs");

            migrationBuilder.DropIndex(
                name: "IX_MovementLogs_CheckersGameStateId",
                table: "MovementLogs");

            migrationBuilder.DropColumn(
                name: "CheckersGameStateId",
                table: "MovementLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckersGameStateId",
                table: "MovementLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_CheckersGameStateId",
                table: "MovementLogs",
                column: "CheckersGameStateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementLogs_CheckersGameStates_CheckersGameStateId",
                table: "MovementLogs",
                column: "CheckersGameStateId",
                principalTable: "CheckersGameStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
