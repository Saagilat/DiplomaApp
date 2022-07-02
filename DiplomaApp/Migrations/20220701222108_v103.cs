using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaApp.Migrations
{
    public partial class v103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeNextPageUrl",
                table: "CategoryMap");

            migrationBuilder.DropColumn(
                name: "AttributeUrl",
                table: "CategoryMap");

            migrationBuilder.DropColumn(
                name: "AttributeUrl",
                table: "CatalogMap");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "6bf5ff05-99ce-4e58-8c62-ef505f129346");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttributeNextPageUrl",
                table: "CategoryMap",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttributeUrl",
                table: "CategoryMap",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttributeUrl",
                table: "CatalogMap",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c811baed-e7a0-4109-b11e-327b6b9333e3");
        }
    }
}
