using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaFon.FileManager.Infrastructure.Migrations
{
    public partial class ChangeDirectory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Directories_DirectoryName",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_DirectoryName",
                table: "Files");
        }
    }
}
