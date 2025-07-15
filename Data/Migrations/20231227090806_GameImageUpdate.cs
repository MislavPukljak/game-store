using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class GameImageUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<List<string>>(
            name: "Images",
            table: "Games",
            type: "text[]",
            nullable: true,
            oldClrType: typeof(List<string>),
            oldType: "text[]");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<List<string>>(
            name: "Images",
            table: "Games",
            type: "text[]",
            nullable: false,
            oldClrType: typeof(List<string>),
            oldType: "text[]",
            oldNullable: true);
    }
}
