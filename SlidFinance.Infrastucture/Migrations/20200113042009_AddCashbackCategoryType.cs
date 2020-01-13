using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddCashbackCategoryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CashbackLimit",
                table: "CashbackCategories",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CashbackCategories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashbackLimit",
                table: "CashbackCategories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CashbackCategories");
        }
    }
}
