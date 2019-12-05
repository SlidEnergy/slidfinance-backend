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

			services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

			services.AddScoped<IRepository<ApplicationUser, string>, EfRepository<ApplicationUser, string>>();
			services.AddScoped<IRepository<Bank, int>, EfRepository<Bank, int>>();
			services.AddScoped<IRepository<Category, int>, EfRepository<Category, int>>();
			services.AddScoped<IRepository<BankAccount, int>, EfRepository<BankAccount, int>>();
			services.AddScoped<IRepository<Rule, int>, EfRepository<Rule, int>>();
			services.AddScoped<IRepository<Transaction, int>, EfRepository<Transaction, int>>();
			services.AddScoped<IAuthTokensRepository, EfAuthTokensRepository>();
			services.AddScoped<IRepository<Mcc, int>, EfRepository<Mcc, int>>();

			services.AddScoped<DataAccessLayer>();

			return services;
		}
	}
}
