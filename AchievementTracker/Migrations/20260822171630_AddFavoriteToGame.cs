using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Game",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Game");
        }
    }
}
