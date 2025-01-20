using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientConnectionId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HubClientConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    ContinueSession = table.Column<bool>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubClientConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HubClientConnections_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubClientConnections_SessionId",
                table: "HubClientConnections",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubClientConnections");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "ClientConnectionId",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Sessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
