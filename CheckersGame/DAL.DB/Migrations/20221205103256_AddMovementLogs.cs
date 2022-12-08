using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddMovementLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "MovementLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckersGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovementFromId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovementToId = table.Column<int>(type: "INTEGER", nullable: false),
                    WhoMoved = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovementLogs_CheckersGames_CheckersGameId",
                        column: x => x.CheckersGameId,
                        principalTable: "CheckersGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovementLogs_DbCoordinates_MovementFromId",
                        column: x => x.MovementFromId,
                        principalTable: "DbCoordinates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovementLogs_DbCoordinates_MovementToId",
                        column: x => x.MovementToId,
                        principalTable: "DbCoordinates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_CheckersGameId",
                table: "MovementLogs",
                column: "CheckersGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_MovementFromId",
                table: "MovementLogs",
                column: "MovementFromId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementLogs_MovementToId",
                table: "MovementLogs",
                column: "MovementToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovementLogs");

            migrationBuilder.DropTable(
                name: "DbCoordinates");
        }
    }
}
