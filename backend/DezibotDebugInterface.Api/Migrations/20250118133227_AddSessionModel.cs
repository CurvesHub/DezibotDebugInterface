using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dezibots_Ip",
                table: "Dezibots");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "Dezibots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ClientConnectionId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dezibots_Ip",
                table: "Dezibots",
                column: "Ip");

            migrationBuilder.CreateIndex(
                name: "IX_Dezibots_SessionId",
                table: "Dezibots",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dezibots_Sessions_SessionId",
                table: "Dezibots",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dezibots_Sessions_SessionId",
                table: "Dezibots");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Dezibots_Ip",
                table: "Dezibots");

            migrationBuilder.DropIndex(
                name: "IX_Dezibots_SessionId",
                table: "Dezibots");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Dezibots");

            migrationBuilder.CreateIndex(
                name: "IX_Dezibots_Ip",
                table: "Dezibots",
                column: "Ip",
                unique: true);
        }
    }
}
