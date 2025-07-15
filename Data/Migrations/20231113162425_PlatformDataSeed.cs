using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations;

/// <inheritdoc />
public partial class PlatformDataSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Platforms",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { 1, "console" },
                { 2, "mobile" },
                { 3, "browser" },
                { 4, "desktop" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Platforms",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "Id", "Category", "Subcategory" },
            values: new object[,]
            {
                { 1, "Races", "Rally" },
                { 3, "Puzzle & Skill", "Puzzle & Skill" },
                { 5, "Strategy", "RTS" },
                { 6, "Action", "TPS" },
                { 7, "Misc.", "Misc." },
                { 8, "RPG", "RPG" },
                { 9, "Adventure", "Adventure" },
                { 13, "Sports", "Sports" },
            });
    }
}
