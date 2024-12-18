using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLogLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogLevel",
                table: "Logs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogLevel",
                table: "Logs");
        }
    }
}
