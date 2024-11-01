using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "CompanyId", "Address", "Country", "Name" },
                values: new object[,]
                {
                    { new Guid("41ca47d3-0c31-47c4-bb61-7aad1cca61e5"), "312 Forest Avenue, BF 923", "USA", "Admin_Solutions Ltd" },
                    { new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d"), "583 Wall Dr. Gwyn Oak, MD 21207", "USA", "IT_Soluctions Ltd" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "Age", "CompanyId", "Name", "Position" },
                values: new object[,]
                {
                    { new Guid("022c78e3-7699-4af9-91c3-85dcf66d1d3a"), 30, new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d"), "Jana Mcleaf", "software developer" },
                    { new Guid("1ec472a4-cb64-439e-968e-8b1d4f7aa2ea"), 26, new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d"), "Sam Raiden", "software developer" },
                    { new Guid("270fa6f5-bb6b-420f-a025-2b8d13007f40"), 35, new Guid("41ca47d3-0c31-47c4-bb61-7aad1cca61e5"), "Kane Miller", "Administrator" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: new Guid("022c78e3-7699-4af9-91c3-85dcf66d1d3a"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: new Guid("1ec472a4-cb64-439e-968e-8b1d4f7aa2ea"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: new Guid("270fa6f5-bb6b-420f-a025-2b8d13007f40"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("41ca47d3-0c31-47c4-bb61-7aad1cca61e5"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d"));
        }
    }
}
