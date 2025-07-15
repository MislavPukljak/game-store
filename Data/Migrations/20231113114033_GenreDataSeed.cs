using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations;

/// <inheritdoc />
public partial class GenreDataSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Name",
            table: "Genres",
            newName: "Subcategory");

        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "Genres",
            type: "text",
            nullable: false,
            defaultValue: string.Empty);

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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 11);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 12);

        migrationBuilder.DeleteData(
            table: "Genres",
            keyColumn: "Id",
            keyValue: 12);

        migrationBuilder.DropColumn(
            name: "Category",
            table: "Genres");

        migrationBuilder.RenameColumn(
            name: "Subcategory",
            table: "Genres",
            newName: "Name");
    }
}
