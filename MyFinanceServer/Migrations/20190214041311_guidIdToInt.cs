using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MyFinanceServer.Migrations
{
    public partial class guidIdToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id2",
                table: "Transactions",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            migrationBuilder.AddColumn<int>(
                name: "CategoryId2",
                table: "Transactions",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "AccountId2",
                table: "Transactions",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                    name: "Id2",
                    table: "Rules",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            migrationBuilder.AddColumn<int>(
                name: "CategoryId2",
                table: "Rules",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "AccountId2",
                table: "Rules",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                    name: "Id2",
                    table: "Categories",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            migrationBuilder.AddColumn<int>(
                    name: "Id2",
                    table: "Banks",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            migrationBuilder.AddColumn<int>(
                    name: "Id2",
                    table: "Accounts",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            migrationBuilder.AddColumn<int>(
                name: "BankId2",
                table: "Accounts",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE public.\"Transactions\" SET \"CategoryId2\" = (SELECT \"Id2\" FROM public.\"Categories\" WHERE public.\"Categories\".\"Id\" = public.\"Transactions\".\"CategoryId\")");
            migrationBuilder.Sql(
                "UPDATE public.\"Transactions\" SET \"AccountId2\" = (SELECT \"Id2\" FROM public.\"Accounts\" WHERE public.\"Accounts\".\"Id\" = public.\"Transactions\".\"AccountId\")");
            migrationBuilder.Sql(
                "UPDATE public.\"Rules\" SET \"CategoryId2\" = (SELECT \"Id2\" FROM public.\"Categories\" WHERE public.\"Categories\".\"Id\" = public.\"Rules\".\"CategoryId\")");
            migrationBuilder.Sql(
                "UPDATE public.\"Rules\" SET \"AccountId2\" = (SELECT \"Id2\" FROM public.\"Accounts\" WHERE public.\"Accounts\".\"Id\" = public.\"Rules\".\"AccountId\")");
            migrationBuilder.Sql(
                "UPDATE public.\"Accounts\" SET \"BankId2\" = (SELECT \"Id2\" FROM public.\"Banks\" WHERE public.\"Banks\".\"Id\" = public.\"Accounts\".\"BankId\")");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId2",
                table: "Transactions",
                nullable: false,
                oldNullable: true);
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId2",
                table: "Rules",
                nullable: true,
                oldNullable: false);
            migrationBuilder.AlterColumn<int>(
                name: "BankId2",
                table: "Accounts",
                nullable: false,
                oldNullable: true);


            migrationBuilder.DropIndex(
                name: "IX_Rules_AccountId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_CategoryId",
                table: "Rules");
            migrationBuilder.DropIndex(
                name: "IX_Accounts_BankId",
                table: "Accounts");
            migrationBuilder.DropIndex(
                name: "IX_Banks_UserId",
                table: "Banks");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions");
            migrationBuilder.DropForeignKey("FK_Accounts_Banks_BankId", "Accounts");
            migrationBuilder.DropForeignKey("FK_Transactions_Accounts_AccountId", "Transactions");
            migrationBuilder.DropForeignKey("FK_Transactions_Categories_CategoryId", "Transactions");
            migrationBuilder.DropForeignKey("FK_Rules_Accounts_AccountId", "Rules");
            migrationBuilder.DropForeignKey("FK_Rules_Categories_CategoryId", "Rules");
            migrationBuilder.DropPrimaryKey("PK_Transactions", "Transactions");
            migrationBuilder.DropPrimaryKey("PK_Accounts", "Accounts");
            migrationBuilder.DropPrimaryKey("PK_Categories", "Categories");
            migrationBuilder.DropPrimaryKey("PK_Banks", "Banks");
            migrationBuilder.DropPrimaryKey("PK_Rules", "Rules");

            migrationBuilder.DropColumn("Id", "Transactions");
            migrationBuilder.DropColumn("Id", "Accounts");
            migrationBuilder.DropColumn("Id", "Categories");
            migrationBuilder.DropColumn("Id", "Banks");
            migrationBuilder.DropColumn("Id", "Rules");

            migrationBuilder.DropColumn("CategoryId", "Transactions");
            migrationBuilder.DropColumn("AccountId", "Transactions");
            migrationBuilder.DropColumn("BankId", "Accounts");
            migrationBuilder.DropColumn("CategoryId", "Rules");
            migrationBuilder.DropColumn("AccountId", "Rules");

            migrationBuilder.RenameColumn("Id2", "Transactions", "Id");
            migrationBuilder.RenameColumn("Id2", "Accounts", "Id");
            migrationBuilder.RenameColumn("Id2", "Categories", "Id");
            migrationBuilder.RenameColumn("Id2", "Banks", "Id");
            migrationBuilder.RenameColumn("Id2", "Rules", "Id");

            migrationBuilder.RenameColumn("CategoryId2", "Transactions", "CategoryId");
            migrationBuilder.RenameColumn("AccountId2", "Transactions", "AccountId");
            migrationBuilder.RenameColumn("BankId2", "Accounts", "BankId");
            migrationBuilder.RenameColumn("CategoryId2", "Rules", "CategoryId");
            migrationBuilder.RenameColumn("AccountId2", "Rules", "AccountId");

            migrationBuilder.AddPrimaryKey("PK_Transactions", "Transactions", "Id");
            migrationBuilder.AddPrimaryKey("PK_Accounts", "Accounts", "Id");
            migrationBuilder.AddPrimaryKey("PK_Categories", "Categories", "Id");
            migrationBuilder.AddPrimaryKey("PK_Banks", "Banks", "Id");
            migrationBuilder.AddPrimaryKey("PK_Rules", "Rules", "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Accounts_AccountId",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict,
                table: "Rules");
            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Categories_CategoryId",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade,
                table: "Rules");
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_AccountId",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade,
                table: "Transactions");
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict,
                table: "Transactions");
            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Banks_BankId",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade,
                table: "Accounts"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Rules_AccountId",
                table: "Rules",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_CategoryId",
                table: "Rules",
                column: "CategoryId");
            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BankId",
                table: "Accounts",
                column: "BankId");
            migrationBuilder.CreateIndex(
                name: "IX_Banks_UserId",
                table: "Banks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
