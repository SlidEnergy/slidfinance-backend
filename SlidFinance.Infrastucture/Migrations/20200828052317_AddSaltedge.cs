using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SlidFinance.WebApi.Migrations
{
    public partial class AddSaltedge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaltedgeAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaltedgeAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrusteeSaltedgeAccounts",
                columns: table => new
                {
                    TrusteeId = table.Column<int>(nullable: false),
                    SaltedgeAccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrusteeSaltedgeAccounts", x => new { x.SaltedgeAccountId, x.TrusteeId });
                    table.ForeignKey(
                        name: "FK_TrusteeSaltedgeAccounts_SaltedgeAccounts_SaltedgeAccountId",
                        column: x => x.SaltedgeAccountId,
                        principalTable: "SaltedgeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrusteeSaltedgeAccounts_Trustee_TrusteeId",
                        column: x => x.TrusteeId,
                        principalTable: "Trustee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrusteeSaltedgeAccounts_TrusteeId",
                table: "TrusteeSaltedgeAccounts",
                column: "TrusteeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrusteeSaltedgeAccounts");

            migrationBuilder.DropTable(
                name: "SaltedgeAccounts");
        }
    }
}
