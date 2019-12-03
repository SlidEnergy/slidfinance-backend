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
			services.AddScoped<AccountsService>();
			services.AddScoped<BanksService>();
			services.AddScoped<RulesService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<CategoriesService>();
			services.AddScoped<ITransactionsService, TransactionsService>();
			services.AddScoped<IMccService, MccService>();
			services.AddScoped<ICategoryStatisticService, CategoryStatisticService>();
			services.AddScoped<IAuthTokenService, AuthTokenService>();

			return services;
		}
	}
}
