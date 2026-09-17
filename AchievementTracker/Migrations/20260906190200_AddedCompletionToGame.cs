using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddedCompletionToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Game",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Game",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Game");
        }
    }
}
