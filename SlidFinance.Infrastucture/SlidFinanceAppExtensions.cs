using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.Infrastructure
{
	public static class SlidFinanceAppExtensions
	{
		public static IServiceCollection AddSlidFinanceInfrastructure(this IServiceCollection services, string connectionString)
		{
			services.AddEntityFrameworkNpgsql()
							.AddDbContext<ApplicationDbContext>(options => options
								.UseLazyLoadingProxies()
								.UseNpgsql(connectionString))
							.BuildServiceProvider();

			services.AddScoped<IRepository<ApplicationUser, string>, EfRepository<ApplicationUser, string>>();
			services.AddScoped<IRepositoryWithAccessCheck<Bank>, EfBanksRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Category>, EfCategoriesRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<BankAccount>, EfBankAccountsRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Rule>, EfRulesRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Transaction>, EfTransactionsRepository>();
			services.AddScoped<IAuthTokensRepository, EfAuthTokensRepository>();
			services.AddScoped<IRepository<Mcc, int>, EfRepository<Mcc, int>>();

			services.AddScoped<DataAccessLayer>();

			return services;
		}
	}
}
