using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DezibotDebugInterface.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeValue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimestampUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassProperty",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertiesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassProperty", x => new { x.ClassId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_ClassProperty_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassProperty_Property_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dezibots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ip = table.Column<string>(type: "TEXT", nullable: false),
                    LastConnectionUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dezibots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dezibots_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionClientConnection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReceiveUpdates = table.Column<bool>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionClientConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionClientConnection_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionClientConnection_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTimeValue",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValuesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTimeValue", x => new { x.PropertyId, x.ValuesId });
                    table.ForeignKey(
                        name: "FK_PropertyTimeValue_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyTimeValue_TimeValue_ValuesId",
                        column: x => x.ValuesId,
                        principalTable: "TimeValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassDezibot",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "INTEGER", nullable: false),
                    DezibotId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDezibot", x => new { x.ClassesId, x.DezibotId });
                    table.ForeignKey(
                        name: "FK_ClassDezibot_Class_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassDezibot_Dezibots_DezibotId",
                        column: x => x.DezibotId,
                        principalTable: "Dezibots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimestampUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LogLevel = table.Column<string>(type: "TEXT", nullable: false),
                    ClassName = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    DezibotId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogEntry_Dezibots_DezibotId",
                        column: x => x.DezibotId,
                        principalTable: "Dezibots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassDezibot_DezibotId",
                table: "ClassDezibot",
                column: "DezibotId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassProperty_PropertiesId",
                table: "ClassProperty",
                column: "PropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Dezibots_Ip",
                table: "Dezibots",
                column: "Ip");

            migrationBuilder.CreateIndex(
                name: "IX_Dezibots_SessionId",
                table: "Dezibots",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_DezibotId",
                table: "LogEntry",
                column: "DezibotId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTimeValue_ValuesId",
                table: "PropertyTimeValue",
                column: "ValuesId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClientConnection_ClientId",
                table: "SessionClientConnection",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionClientConnection_SessionId",
                table: "SessionClientConnection",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassDezibot");

            migrationBuilder.DropTable(
                name: "ClassProperty");

            migrationBuilder.DropTable(
                name: "LogEntry");

            migrationBuilder.DropTable(
                name: "PropertyTimeValue");

            migrationBuilder.DropTable(
                name: "SessionClientConnection");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Dezibots");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "TimeValue");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
