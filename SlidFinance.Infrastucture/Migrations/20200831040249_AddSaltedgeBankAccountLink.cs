using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddSaltedgeBankAccountLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaltedgeBankAccountId",
                table: "Accounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaltedgeBankAccountId",
                table: "Accounts");
        }
    }
}
