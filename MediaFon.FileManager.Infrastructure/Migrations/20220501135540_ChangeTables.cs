using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFon.FileManager.Infrastructure.Migrations
{
    public partial class ChangeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalPath",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemotePath",
                table: "Files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocalPath",
                table: "Directories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemotePath",
                table: "Directories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalPath",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "RemotePath",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LocalPath",
                table: "Directories");

            migrationBuilder.DropColumn(
                name: "RemotePath",
                table: "Directories");
        }
    }
}
