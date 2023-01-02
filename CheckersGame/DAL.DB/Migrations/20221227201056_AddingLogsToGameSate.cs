using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddingLogsToGameSate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbCoordinates");

            migrationBuilder.AddColumn<int>(
                name: "MovementLogId",
                table: "CheckersGameStates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CheckersGameStates_MovementLogId",
                table: "CheckersGameStates",
                column: "MovementLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckersGameStates_MovementLogs_MovementLogId",
                table: "CheckersGameStates",
                column: "MovementLogId",
                principalTable: "MovementLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckersGameStates_MovementLogs_MovementLogId",
                table: "CheckersGameStates");

            migrationBuilder.DropIndex(
                name: "IX_CheckersGameStates_MovementLogId",
                table: "CheckersGameStates");

            migrationBuilder.DropColumn(
                name: "MovementLogId",
                table: "CheckersGameStates");

            migrationBuilder.CreateTable(
                name: "DbCoordinates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbCoordinates", x => x.Id);
                });
        }
    }
}
