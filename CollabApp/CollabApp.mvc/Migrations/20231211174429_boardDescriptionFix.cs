using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabApp.mvc.Migrations
{
    /// <inheritdoc />
    public partial class boardDescriptionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoardDescription",
                table: "Boards",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardDescription",
                table: "Boards");
        }
    }
}
