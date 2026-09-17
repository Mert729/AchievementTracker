using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddUnlockedDateToAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UnlockDate",
                table: "Achievement",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnlockDate",
                table: "Achievement");
        }
    }
}
