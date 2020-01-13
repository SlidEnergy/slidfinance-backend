using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class FixMccInCashbackCategoryMcc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mcc",
                table: "CashbackCategoryMcc");

            migrationBuilder.CreateIndex(
                name: "IX_CashbackCategoryMcc_MccId",
                table: "CashbackCategoryMcc",
                column: "MccId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashbackCategoryMcc_Mcc_MccId",
                table: "CashbackCategoryMcc",
                column: "MccId",
                principalTable: "Mcc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashbackCategoryMcc_Mcc_MccId",
                table: "CashbackCategoryMcc");

            migrationBuilder.DropIndex(
                name: "IX_CashbackCategoryMcc_MccId",
                table: "CashbackCategoryMcc");

            migrationBuilder.AddColumn<byte>(
                name: "Mcc",
                table: "CashbackCategoryMcc",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
