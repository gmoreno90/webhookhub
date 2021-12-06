using Microsoft.EntityFrameworkCore.Migrations;

namespace WebHookHub.Migrations
{
    public partial class CustomJobIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomJobIDs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalJobID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InternalJobID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomJobIDs", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomJobIDs_ExternalJobID",
                table: "CustomJobIDs",
                column: "ExternalJobID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomJobIDs");
        }
    }
}
