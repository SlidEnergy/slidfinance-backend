using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App
{
	public static class SlidFinanceAppExtensions
	{
		public static IServiceCollection AddSlidFinanceCore(this IServiceCollection services)
		{
			services.AddScoped<IAccountsService, AccountsService>();
			services.AddScoped<IBanksService, BanksService>();
			services.AddScoped<IRulesService, RulesService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<ICategoriesService, CategoriesService>();
			services.AddScoped<ITransactionsService, TransactionsService>();
			services.AddScoped<IMccService, MccService>();
			services.AddScoped<ICategoryStatisticService, CategoryStatisticService>();
			services.AddScoped<IAuthTokenService, AuthTokenService>();
			services.AddScoped<IImportService, ImportService>();

			return services;
		}
	}
}
