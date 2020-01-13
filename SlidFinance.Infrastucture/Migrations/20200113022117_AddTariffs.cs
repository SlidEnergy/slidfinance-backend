using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddTariffs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ProductTariff_SelectedTariffId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTariff_Products_ProductId",
                table: "ProductTariff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTariff",
                table: "ProductTariff");

            migrationBuilder.RenameTable(
                name: "ProductTariff",
                newName: "Tariffs");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTariff_ProductId",
                table: "Tariffs",
                newName: "IX_Tariffs_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Products",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tariffs",
                table: "Tariffs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Tariffs_SelectedTariffId",
                table: "Accounts",
                column: "SelectedTariffId",
                principalTable: "Tariffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tariffs_Products_ProductId",
                table: "Tariffs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Tariffs_SelectedTariffId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tariffs_Products_ProductId",
                table: "Tariffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tariffs",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Tariffs",
                newName: "ProductTariff");

            migrationBuilder.RenameIndex(
                name: "IX_Tariffs_ProductId",
                table: "ProductTariff",
                newName: "IX_ProductTariff_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTariff",
                table: "ProductTariff",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ProductTariff_SelectedTariffId",
                table: "Accounts",
                column: "SelectedTariffId",
                principalTable: "ProductTariff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTariff_Products_ProductId",
                table: "ProductTariff",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
