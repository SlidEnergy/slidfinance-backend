using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFinanceServer.Migrations
{
    public partial class TransactionMCCBankDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCategory",
                table: "Transactions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Mcc",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Category",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCategory",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Mcc",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Category",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
