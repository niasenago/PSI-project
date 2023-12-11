using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabApp.mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsQuestion",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsQuestion",
                table: "Posts");
        }
    }
}
