using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebHookHub.Migrations
{
    public partial class DataToPostNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "DataToPosts",
                type: "nvarchar(max)",
                maxLength: 500000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 500000);

            migrationBuilder.AddColumn<string>(
                name: "ClientCode",
                table: "DataToPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ContentBinary",
                table: "DataToPosts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "ContentExtraID",
                table: "DataToPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentID",
                table: "DataToPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventCode",
                table: "DataToPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientCode",
                table: "DataToPosts");

            migrationBuilder.DropColumn(
                name: "ContentBinary",
                table: "DataToPosts");

            migrationBuilder.DropColumn(
                name: "ContentExtraID",
                table: "DataToPosts");

            migrationBuilder.DropColumn(
                name: "ContentID",
                table: "DataToPosts");

            migrationBuilder.DropColumn(
                name: "EventCode",
                table: "DataToPosts");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "DataToPosts",
                type: "nvarchar(max)",
                maxLength: 500000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 500000,
                oldNullable: true);
        }
    }
}
