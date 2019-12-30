using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddTrustee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrusteeId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Trustee",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trustee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrusteeAccounts",
                columns: table => new
                {
                    TrusteeId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrusteeAccounts", x => new { x.AccountId, x.TrusteeId });
                    table.ForeignKey(
                        name: "FK_TrusteeAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrusteeAccounts_Trustee_TrusteeId",
                        column: x => x.TrusteeId,
                        principalTable: "Trustee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrusteeCategories",
                columns: table => new
                {
                    TrusteeId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrusteeCategories", x => new { x.CategoryId, x.TrusteeId });
                    table.ForeignKey(
                        name: "FK_TrusteeCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrusteeCategories_Trustee_TrusteeId",
                        column: x => x.TrusteeId,
                        principalTable: "Trustee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

			migrationBuilder.Sql(
				"INSERT INTO \"Trustee\" (\"Id\") SELECT ROW_NUMBER() OVER (ORDER BY \"AspNetUsers\".\"Id\") FROM \"AspNetUsers\"");

			migrationBuilder.Sql(
				"UPDATE \"AspNetUsers\" SET \"TrusteeId\" = T.N FROM (SELECT \"AspNetUsers\".\"Id\", ROW_NUMBER() OVER (ORDER BY \"AspNetUsers\".\"Id\") AS N FROM \"AspNetUsers\") AS T WHERE T.\"Id\" = \"AspNetUsers\".\"Id\"");

			migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TrusteeId",
                table: "AspNetUsers",
                column: "TrusteeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrusteeAccounts_TrusteeId",
                table: "TrusteeAccounts",
                column: "TrusteeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrusteeCategories_TrusteeId",
                table: "TrusteeCategories",
                column: "TrusteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Trustee_TrusteeId",
                table: "AspNetUsers",
                column: "TrusteeId",
                principalTable: "Trustee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

			migrationBuilder.Sql(
			  "INSERT INTO \"TrusteeAccounts\" (\"TrusteeId\", \"AccountId\") SELECT DISTINCT \"AspNetUsers\".\"TrusteeId\", \"Accounts\".\"Id\" FROM \"AspNetUsers\" INNER JOIN \"Banks\" ON \"AspNetUsers\".\"Id\" = \"Banks\".\"UserId\" INNER JOIN \"Accounts\" ON public.\"Banks\".\"Id\" = \"Accounts\".\"BankId\"");

			migrationBuilder.Sql(
			  "INSERT INTO \"TrusteeCategories\" (\"TrusteeId\", \"CategoryId\") SELECT DISTINCT \"AspNetUsers\".\"TrusteeId\", \"Categories\".\"Id\" FROM \"AspNetUsers\" INNER JOIN \"Categories\" ON \"AspNetUsers\".\"Id\" = \"Categories\".\"UserId\"");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Trustee_TrusteeId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TrusteeAccounts");

            migrationBuilder.DropTable(
                name: "TrusteeCategories");

            migrationBuilder.DropTable(
                name: "Trustee");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TrusteeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TrusteeId",
                table: "AspNetUsers");
        }
    }
}
