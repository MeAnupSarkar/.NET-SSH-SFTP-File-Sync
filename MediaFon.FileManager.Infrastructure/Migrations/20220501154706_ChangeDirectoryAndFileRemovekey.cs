using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFon.FileManager.Infrastructure.Migrations
{
    public partial class ChangeDirectoryAndFileRemovekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Directories_DirectoryName",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_DirectoryName",
                table: "Files");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Directories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Directories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Directories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Directories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Directories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Directories");

            migrationBuilder.CreateIndex(
                name: "IX_Files_DirectoryName",
                table: "Files",
                column: "DirectoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Directories_DirectoryName",
                table: "Files",
                column: "DirectoryName",
                principalTable: "Directories",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
