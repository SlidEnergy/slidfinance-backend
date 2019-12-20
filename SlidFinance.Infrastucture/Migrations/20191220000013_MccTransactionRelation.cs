using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class MccTransactionRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_AspNetUsers_UserId",
                table: "Banks");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Banks_UserId",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "Mcc",
                table: "Transactions",
                newName: "MccId");

            migrationBuilder.Sql(
                "UPDATE \"Transactions\" SET \"MccId\" = CASE WHEN EXISTS (SELECT \"MccId\" FROM \"Mcc\" WHERE CAST(\"Mcc\".\"Code\" AS integer) = \"MccId\") THEN (SELECT \"Id\" FROM \"Mcc\" WHERE CAST(\"Mcc\".\"Code\" AS integer) = \"MccId\" LIMIT 1) ELSE NULL END");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MccId",
                table: "Transactions",
                column: "MccId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Mcc_MccId",
                table: "Transactions",
                column: "MccId",
                principalTable: "Mcc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Mcc_MccId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_MccId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "MccId",
                table: "Transactions",
                newName: "Mcc");
        }
    }
}
