using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Net_Core_Identity_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1fa027fb-589b-4a7b-85e7-ed7eada028f9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36236ce7-9777-470a-99f8-9060e35d9ee4");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "48b2ee19-e154-4da5-a639-b1a1e6d927cb", null, "User", "USER" },
                    { "bcb35e99-7435-4a9e-a8fd-bab300d34d6b", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48b2ee19-e154-4da5-a639-b1a1e6d927cb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcb35e99-7435-4a9e-a8fd-bab300d34d6b");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1fa027fb-589b-4a7b-85e7-ed7eada028f9", null, "User", "USER" },
                    { "36236ce7-9777-470a-99f8-9060e35d9ee4", null, "Admin", "ADMIN" }
                });
        }
    }
}
