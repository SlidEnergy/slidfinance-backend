using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
	public interface IApplicationDbContext
	{
		DbSet<BankAccount> Accounts { get; set; }
		DbSet<AuthToken> AuthTokens { get; set; }
		DbSet<Bank> Banks { get; set; }
		DbSet<Category> Categories { get; set; }
		DbSet<Mcc> Mcc { get; set; }
		DbSet<IMerchant> Merchants { get; set; }
		DbSet<Rule> Rules { get; set; }
		DbSet<Transaction> Transactions { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}