using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaApp.Migrations
{
    public partial class v103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "OfferPage");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "CategoryPage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "OfferPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "CategoryPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
