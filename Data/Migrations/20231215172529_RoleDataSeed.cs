using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations;

/// <inheritdoc />
public partial class RoleDataSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            values: new object[,]
            {
                { "36ec611c-439f-4aa8-ab05-8815f40f2e6a", null, "Administrator", "ADMINISTRATOR" },
                { "471212d6-bf24-46ee-96c2-a5699cd38f76", null, "Moderator", "MODERATOR" },
                { "bc7b40eb-e1be-409c-86dd-751fae843c3c", null, "Publisher", "PUBLISHER" },
                { "c1997f9c-1c85-42e0-8f28-ff64ff7b7dae", null, "Manager", "MANAGER" },
                { "d52cfe31-8d3b-4a74-ac2b-b72798a794eb", null, "User", "USER" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "36ec611c-439f-4aa8-ab05-8815f40f2e6a");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "471212d6-bf24-46ee-96c2-a5699cd38f76");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "bc7b40eb-e1be-409c-86dd-751fae843c3c");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "c1997f9c-1c85-42e0-8f28-ff64ff7b7dae");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "d52cfe31-8d3b-4a74-ac2b-b72798a794eb");
    }
}
