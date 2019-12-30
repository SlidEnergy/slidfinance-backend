using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class RuleMccCodeToMccId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mcc",
                table: "Rules",
                newName: "MccId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_MccId",
                table: "Rules",
                column: "MccId");

            migrationBuilder.Sql(
                "UPDATE \"Rules\" SET \"MccId\" = CASE WHEN EXISTS (SELECT \"MccId\" FROM \"Mcc\" WHERE CAST(\"Mcc\".\"Code\" AS integer) = \"MccId\") THEN (SELECT \"Id\" FROM \"Mcc\" WHERE CAST(\"Mcc\".\"Code\" AS integer) = \"MccId\" LIMIT 1) ELSE NULL END");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Mcc_MccId",
                table: "Rules",
                column: "MccId",
                principalTable: "Mcc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Mcc_MccId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_MccId",
                table: "Rules");

            migrationBuilder.RenameColumn(
                name: "MccId",
                table: "Rules",
                newName: "Mcc");
        }
    }
}
