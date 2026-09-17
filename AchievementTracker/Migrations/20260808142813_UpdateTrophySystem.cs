using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrophySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Achievement",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DifficultyLevel",
                table: "Achievement",
                newName: "TrophyTierId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Game",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrophyTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrophyTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementTags",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTags", x => new { x.AchievementId, x.TagId });
                    table.ForeignKey(
                        name: "FK_AchievementTags_Achievement_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AchievementTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievement_TrophyTierId",
                table: "Achievement",
                column: "TrophyTierId");

            migrationBuilder.CreateIndex(
                name: "IX_AchievementTags_TagId",
                table: "AchievementTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_TrophyTiers_TrophyTierId",
                table: "Achievement",
                column: "TrophyTierId",
                principalTable: "TrophyTiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievement_TrophyTiers_TrophyTierId",
                table: "Achievement");

            migrationBuilder.DropTable(
                name: "AchievementTags");

            migrationBuilder.DropTable(
                name: "TrophyTiers");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Achievement_TrophyTierId",
                table: "Achievement");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Game");

            migrationBuilder.RenameColumn(
                name: "TrophyTierId",
                table: "Achievement",
                newName: "DifficultyLevel");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Achievement",
                newName: "Title");
        }
    }
}
