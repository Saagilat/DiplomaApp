using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaApp.Migrations
{
    public partial class v102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c811baed-e7a0-4109-b11e-327b6b9333e3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d266ab51-de30-4d95-b122-8a6810e2c27e");
        }
    }
}
