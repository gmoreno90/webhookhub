using Microsoft.EntityFrameworkCore.Migrations;

namespace WebHookHub.Migrations
{
    public partial class RegexFindIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegexID",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegexIDExtra",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegexID",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RegexIDExtra",
                table: "Events");
        }
    }
}
