using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class ImageDataTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Images",
            table: "Games");

        migrationBuilder.CreateTable(
            name: "ImagesData",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Container = table.Column<string>(type: "text", nullable: false),
                ContentType = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ImagesData", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GameImagesData",
            columns: table => new
            {
                GamesId = table.Column<int>(type: "integer", nullable: false),
                ImagesId = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GameImagesData", x => new { x.GamesId, x.ImagesId });
                table.ForeignKey(
                    name: "FK_GameImagesData_Games_GamesId",
                    column: x => x.GamesId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GameImagesData_ImagesData_ImagesId",
                    column: x => x.ImagesId,
                    principalTable: "ImagesData",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GameImagesData_ImagesId",
            table: "GameImagesData",
            column: "ImagesId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GameImagesData");

        migrationBuilder.DropTable(
            name: "ImagesData");

        migrationBuilder.AddColumn<List<string>>(
            name: "Images",
            table: "Games",
            type: "text[]",
            nullable: true);
    }
}
