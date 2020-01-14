using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class RemoveCashbackMccReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashbackCategoryMcc_Mcc_MccId",
                table: "CashbackCategoryMcc");

            migrationBuilder.DropIndex(
                name: "IX_CashbackCategoryMcc_MccId",
                table: "CashbackCategoryMcc");

            migrationBuilder.RenameColumn(
                name: "MccId",
                table: "CashbackCategoryMcc",
                newName: "MccCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MccCode",
                table: "CashbackCategoryMcc",
                newName: "MccId");

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
    }
}
