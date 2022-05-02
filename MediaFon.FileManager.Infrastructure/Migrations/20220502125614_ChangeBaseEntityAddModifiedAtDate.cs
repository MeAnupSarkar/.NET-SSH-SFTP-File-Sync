using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFon.FileManager.Infrastructure.Migrations
{
    public partial class ChangeBaseEntityAddModifiedAtDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Files",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "EventLogs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Directories",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Directories");
        }
    }
}
