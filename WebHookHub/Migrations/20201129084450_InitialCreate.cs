using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace WebHookHub.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiLogItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    ResponseMillis = table.Column<long>(nullable: false),
                    StatusCode = table.Column<int>(nullable: false),
                    Method = table.Column<string>(maxLength: 500, nullable: false),
                    Path = table.Column<string>(maxLength: 1000, nullable: false),
                    QueryString = table.Column<string>(maxLength: 5000, nullable: true),
                    RequestBody = table.Column<string>(maxLength: 50000, nullable: true),
                    ResponseBody = table.Column<string>(maxLength: 50000, nullable: true),
                    RequestToken = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLogItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ClientEvents",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    PostUrl = table.Column<string>(maxLength: 5000, nullable: false),
                    UserName = table.Column<string>(maxLength: 500, nullable: true),
                    PassWord = table.Column<string>(maxLength: 500, nullable: true),
                    Enable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEvents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ClientEvents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvents_ClientId",
                table: "ClientEvents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvents_EventId",
                table: "ClientEvents",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiLogItems");

            migrationBuilder.DropTable(
                name: "ClientEvents");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
