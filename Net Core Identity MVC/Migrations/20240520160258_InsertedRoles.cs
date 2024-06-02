using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Net_Core_Identity_MVC.Migrations
{
    /// <inheritdoc />
    public partial class InsertedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11af5c5f-d237-4049-b74c-31789eb03eec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3191e609-0b6c-48ee-9eb8-9d22060f2a6d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1fa027fb-589b-4a7b-85e7-ed7eada028f9", null, "User", "USER" },
                    { "36236ce7-9777-470a-99f8-9060e35d9ee4", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1fa027fb-589b-4a7b-85e7-ed7eada028f9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36236ce7-9777-470a-99f8-9060e35d9ee4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11af5c5f-d237-4049-b74c-31789eb03eec", null, "User", "USER" },
                    { "3191e609-0b6c-48ee-9eb8-9d22060f2a6d", null, "Admin", "ADMIN" }
                });
        }
    }
}
