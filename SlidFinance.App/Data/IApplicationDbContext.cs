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
		DbSet<Models.Merchant> Merchants { get; set; }
		DbSet<Rule> Rules { get; set; }
		DbSet<Transaction> Transactions { get; set; }
		DbSet<TrusteeAccount> TrusteeAccounts { get; set; }
		DbSet<TrusteeCategory> TrusteeCategories { get; set; }
		DbSet<TrusteeProduct> TrusteeProducts { get; set; }
		DbSet<ApplicationUser> Users { get; set; }
		DbSet<Product> Products { get; set; }
		DbSet<ProductTariff> Tariffs { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}