using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddCashback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashbackCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Title = table.Column<string>(nullable: true),
                    TariffId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashbackCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashbackCategories_Tariffs_TariffId",
                        column: x => x.TariffId,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashbackCategoryMcc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    MccId = table.Column<int>(nullable: false),
                    Mcc = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashbackCategoryMcc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashbackCategoryMcc_CashbackCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CashbackCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashbackCategories_TariffId",
                table: "CashbackCategories",
                column: "TariffId");

            migrationBuilder.CreateIndex(
                name: "IX_CashbackCategoryMcc_CategoryId",
                table: "CashbackCategoryMcc",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashbackCategoryMcc");

            migrationBuilder.DropTable(
                name: "CashbackCategories");
        }
    }
}
