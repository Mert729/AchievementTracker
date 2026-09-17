using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementTracker.Migrations
{
    /// <inheritdoc />
    public partial class Sprint2_Database_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Achievement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpoiler",
                table: "Achievement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Achievement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Achievement");

            migrationBuilder.DropColumn(
                name: "IsSpoiler",
                table: "Achievement");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Achievement");
        }
    }
}
