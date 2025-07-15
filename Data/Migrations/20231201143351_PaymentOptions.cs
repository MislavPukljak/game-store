using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations;

/// <inheritdoc />
public partial class PaymentOptions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "PaymentOptions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentOptions", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "PaymentOptions",
            columns: new[] { "Id", "Description", "Title" },
            values: new object[,]
            {
                { 1, "IBox account", "IBox terminal" },
                { 2, "Download invoce.", "Bank" },
                { 3, "Enter card details", "Visa" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PaymentOptions");
    }
}
