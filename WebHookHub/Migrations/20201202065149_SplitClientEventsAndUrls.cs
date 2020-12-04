using Microsoft.EntityFrameworkCore.Migrations;

namespace WebHookHub.Migrations
{
    public partial class SplitClientEventsAndUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassWord",
                table: "ClientEvents");

            migrationBuilder.DropColumn(
                name: "PostUrl",
                table: "ClientEvents");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ClientEvents");

            migrationBuilder.AddColumn<string>(
                name: "ContentReponseError",
                table: "ClientEvents",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentReponseOk",
                table: "ClientEvents",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientEventWebhooks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientEventId = table.Column<int>(type: "int", nullable: false),
                    PostUrl = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PassWord = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Enable = table.Column<bool>(type: "bit", nullable: false),
                    ExpectedContentResult = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEventWebhooks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ClientEventWebhooks_ClientEvents_ClientEventId",
                        column: x => x.ClientEventId,
                        principalTable: "ClientEvents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEventWebhooks_ClientEventId",
                table: "ClientEventWebhooks",
                column: "ClientEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEventWebhooks");

            migrationBuilder.DropColumn(
                name: "ContentReponseError",
                table: "ClientEvents");

            migrationBuilder.DropColumn(
                name: "ContentReponseOk",
                table: "ClientEvents");

            migrationBuilder.AddColumn<string>(
                name: "PassWord",
                table: "ClientEvents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostUrl",
                table: "ClientEvents",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ClientEvents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
