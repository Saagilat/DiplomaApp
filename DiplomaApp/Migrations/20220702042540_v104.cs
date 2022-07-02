using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaApp.Migrations
{
    public partial class v104 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "5e76cee0-48f0-45d5-8e92-1d886fa6008b");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "6bf5ff05-99ce-4e58-8c62-ef505f129346");
        }
    }
}
