using Microsoft.EntityFrameworkCore.Migrations;

namespace WebHookHub.Migrations
{
    public partial class GenericHeaderAutentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassWord",
                table: "ClientEventWebhooks");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ClientEventWebhooks");

            migrationBuilder.AddColumn<string>(
                name: "HeaderAuthorizationValue",
                table: "ClientEventWebhooks",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderAuthorizationValue",
                table: "ClientEventWebhooks");

            migrationBuilder.AddColumn<string>(
                name: "PassWord",
                table: "ClientEventWebhooks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ClientEventWebhooks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
