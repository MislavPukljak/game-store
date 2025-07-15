using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddTablesFromMongo : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "Discount",
            table: "OrderDetails",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(float),
            oldType: "real");

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EmployeeID = table.Column<int>(type: "integer", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: false),
                Title = table.Column<string>(type: "text", nullable: false),
                TitleOfCourtesy = table.Column<string>(type: "text", nullable: false),
                BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                HireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Address = table.Column<string>(type: "text", nullable: false),
                City = table.Column<string>(type: "text", nullable: false),
                Region = table.Column<string>(type: "text", nullable: false),
                PostalCode = table.Column<string>(type: "text", nullable: false),
                Country = table.Column<string>(type: "text", nullable: false),
                HomePhone = table.Column<string>(type: "text", nullable: false),
                Extension = table.Column<string>(type: "text", nullable: false),
                Photo = table.Column<byte[]>(type: "bytea", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: false),
                ReportsTo = table.Column<string>(type: "text", nullable: false),
                PhotoPath = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EmployeeTerritories",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EmployeeID = table.Column<int>(type: "integer", nullable: false),
                TerritoryID = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmployeeTerritories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Regions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RegionID = table.Column<int>(type: "integer", nullable: false),
                RegionDescription = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Regions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Territories",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                TerritoryID = table.Column<int>(type: "integer", nullable: false),
                TerritoryDescription = table.Column<string>(type: "text", nullable: false),
                RegionID = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Territories", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Employees");

        migrationBuilder.DropTable(
            name: "EmployeeTerritories");

        migrationBuilder.DropTable(
            name: "Regions");

        migrationBuilder.DropTable(
            name: "Territories");

        migrationBuilder.AlterColumn<float>(
            name: "Discount",
            table: "OrderDetails",
            type: "real",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");
    }
}
