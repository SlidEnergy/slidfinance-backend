using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IApplicationDbContext
	{
		DbSet<BankAccount> Accounts { get; set; }
		DbSet<AuthToken> AuthTokens { get; set; }
		DbSet<Bank> Banks { get; set; }
		DbSet<UserCategory> Categories { get; set; }
		DbSet<Mcc> Mcc { get; set; }
		DbSet<Merchant> Merchants { get; set; }
		DbSet<Rule> Rules { get; set; }
		DbSet<Transaction> Transactions { get; set; }
		DbSet<TrusteeAccount> TrusteeAccounts { get; set; }
		DbSet<TrusteeSaltedgeAccount> TrusteeSaltedgeAccounts { get; set; }
		DbSet<TrusteeCategory> TrusteeCategories { get; set; }
		DbSet<TrusteeProduct> TrusteeProducts { get; set; }
		DbSet<ApplicationUser> Users { get; set; }
		DbSet<Product> Products { get; set; }
		DbSet<ProductTariff> Tariffs { get; set; }
		DbSet<CashbackCategory> CashbackCategories { get; set; }
		DbSet<CashbackCategoryMcc> CashbackCategoryMcc { get; set; }
		DbSet<Cashback> Cashback { get; set; }
		DbSet<SaltedgeAccount> SaltedgeAccounts { get; set; }

		DbSet<IdentityRole> Roles { get; set; }

		DbSet<IdentityUserRole<string>> UserRoles { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}