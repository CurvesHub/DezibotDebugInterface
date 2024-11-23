using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dezibots",
                columns: table => new
                {
                    Ip = table.Column<string>(type: "TEXT", nullable: false),
                    LastConnectionUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dezibots", x => x.Ip);
                });

            migrationBuilder.CreateTable(
                name: "DezibotClasses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DezibotIp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DezibotClasses", x => new { x.DezibotIp, x.Name });
                    table.ForeignKey(
                        name: "FK_DezibotClasses_Dezibots_DezibotIp",
                        column: x => x.DezibotIp,
                        principalTable: "Dezibots",
                        principalColumn: "Ip",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DezibotLogs",
                columns: table => new
                {
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DezibotIp = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DezibotLogs", x => new { x.DezibotIp, x.TimestampUtc });
                    table.ForeignKey(
                        name: "FK_DezibotLogs_Dezibots_DezibotIp",
                        column: x => x.DezibotIp,
                        principalTable: "Dezibots",
                        principalColumn: "Ip",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DezibotClassProperties",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DezibotIp = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DezibotClassProperties", x => new { x.DezibotIp, x.ClassName, x.Name });
                    table.ForeignKey(
                        name: "FK_DezibotClassProperties_DezibotClasses_DezibotIp_ClassName",
                        columns: x => new { x.DezibotIp, x.ClassName },
                        principalTable: "DezibotClasses",
                        principalColumns: new[] { "DezibotIp", "Name" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DezibotPropertyValues",
                columns: table => new
                {
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DezibotIp = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false),
                    PropertyName = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DezibotPropertyValues", x => new { x.DezibotIp, x.ClassName, x.PropertyName, x.TimestampUtc });
                    table.ForeignKey(
                        name: "FK_DezibotPropertyValues_DezibotClassProperties_DezibotIp_ClassName_PropertyName",
                        columns: x => new { x.DezibotIp, x.ClassName, x.PropertyName },
                        principalTable: "DezibotClassProperties",
                        principalColumns: new[] { "DezibotIp", "ClassName", "Name" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DezibotLogs");

            migrationBuilder.DropTable(
                name: "DezibotPropertyValues");

            migrationBuilder.DropTable(
                name: "DezibotClassProperties");

            migrationBuilder.DropTable(
                name: "DezibotClasses");

            migrationBuilder.DropTable(
                name: "Dezibots");
        }
    }
}
