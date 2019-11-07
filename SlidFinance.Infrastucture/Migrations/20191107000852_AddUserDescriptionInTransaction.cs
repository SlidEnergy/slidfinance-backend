using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddUserDescriptionInTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserDescription",
                table: "Transactions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDescription",
                table: "Transactions");
        }
    }
}
