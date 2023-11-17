using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabApp.mvc.Migrations
{
    /// <inheritdoc />
    public partial class testofgoogle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SavedFileName",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedUrl",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedFileName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SavedUrl",
                table: "Posts");
        }
    }
}
