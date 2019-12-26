using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddMerchantFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Merchants",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Merchants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_CreatedById",
                table: "Merchants",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Merchants_AspNetUsers_CreatedById",
                table: "Merchants",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Merchants_AspNetUsers_CreatedById",
                table: "Merchants");

            migrationBuilder.DropIndex(
                name: "IX_Merchants_CreatedById",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Merchants");
        }
    }
}
