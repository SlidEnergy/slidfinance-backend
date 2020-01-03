using Microsoft.EntityFrameworkCore.Migrations;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddTrusteeProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Product_ProductId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Banks_BankId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTariff_Product_ProductId",
                table: "ProductTariff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_Product_BankId",
                table: "Products",
                newName: "IX_Products_BankId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TrusteeProducts",
                columns: table => new
                {
                    TrusteeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrusteeProducts", x => new { x.ProductId, x.TrusteeId });
                    table.ForeignKey(
                        name: "FK_TrusteeProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrusteeProducts_Trustee_TrusteeId",
                        column: x => x.TrusteeId,
                        principalTable: "Trustee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrusteeProducts_TrusteeId",
                table: "TrusteeProducts",
                column: "TrusteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Products_ProductId",
                table: "Accounts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Banks_BankId",
                table: "Products",
                column: "BankId",
                principalTable: "Banks",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Products_ProductId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Banks_BankId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTariff_Products_ProductId",
                table: "ProductTariff");

            migrationBuilder.DropTable(
                name: "TrusteeProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameIndex(
                name: "IX_Products_BankId",
                table: "Product",
                newName: "IX_Product_BankId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Product_ProductId",
                table: "Accounts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Banks_BankId",
                table: "Product",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTariff_Product_ProductId",
                table: "ProductTariff",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
