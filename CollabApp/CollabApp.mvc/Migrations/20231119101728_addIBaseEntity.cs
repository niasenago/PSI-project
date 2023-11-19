using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabApp.mvc.Migrations
{
    /// <inheritdoc />
    public partial class addIBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Boards",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "BoardId",
                table: "Boards",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                table: "Boards",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Boards",
                newName: "BoardId");
        }
    }
}
