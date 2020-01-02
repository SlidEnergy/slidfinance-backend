using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddProductsTariffsCashback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Banks_BankId",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "Accounts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedTariffId",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Accounts",
                nullable: false,
                defaultValue: 0);

			migrationBuilder.Sql(
				"UPDATE \"Accounts\" SET \"Type\" = 2");

			migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    BankId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    Approved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTariff",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Title = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTariff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTariff_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ProductId",
                table: "Accounts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_SelectedTariffId",
                table: "Accounts",
                column: "SelectedTariffId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BankId",
                table: "Product",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTariff_ProductId",
                table: "ProductTariff",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Banks_BankId",
                table: "Accounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Product_ProductId",
                table: "Accounts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_ProductTariff_SelectedTariffId",
                table: "Accounts",
                column: "SelectedTariffId",
                principalTable: "ProductTariff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Banks_BankId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Product_ProductId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_ProductTariff_SelectedTariffId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "ProductTariff");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ProductId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_SelectedTariffId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SelectedTariffId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "BankId",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Banks_BankId",
                table: "Accounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
