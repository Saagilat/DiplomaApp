using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomaApp.Migrations
{
    public partial class v102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlMarketplace",
                table: "OfferPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlMarketplace",
                table: "CategoryPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlMarketplace",
                table: "CatalogPage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlMarketplace",
                table: "OfferPage");

            migrationBuilder.DropColumn(
                name: "UrlMarketplace",
                table: "CategoryPage");

            migrationBuilder.DropColumn(
                name: "UrlMarketplace",
                table: "CatalogPage");
        }
    }
}
