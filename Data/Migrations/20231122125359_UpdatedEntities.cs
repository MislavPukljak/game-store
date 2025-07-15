using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class UpdatedEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "Games",
            newName: "UnitInStock");

        migrationBuilder.RenameColumn(
            name: "Discount",
            table: "Games",
            newName: "Discontinued");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UnitInStock",
            table: "Games",
            newName: "Quantity");

        migrationBuilder.RenameColumn(
            name: "Discontinued",
            table: "Games",
            newName: "Discount");
    }
}
