using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabApp.mvc.Migrations
{
    /// <inheritdoc />
    public partial class settingUpBoardPostRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardId",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BoardId",
                table: "Posts",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Board",
                table: "Posts",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "BoardId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Board",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_BoardId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Posts");
        }
    }
}
