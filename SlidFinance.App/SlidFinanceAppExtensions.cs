using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SlidFinance.App.Analysis;
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
			services.AddScoped<IAuthTokenService, AuthTokenService>();
			services.AddScoped<IImportService, ImportService>();
			services.AddScoped<IMerchantService, MerchantService>();
			services.AddScoped<IProductsService, ProductsService>();
			services.AddScoped<IProductTariffsService, ProductTariffsService>();
			services.AddScoped<ICashbackCategoriesService, CashbackCategoriesService>();
			services.AddScoped<ICashbackCategoryMccService, CashbackCategoryMccService>();

			// Analysis

			services.AddScoped<ICashbackService, CashbackService>();
			services.AddScoped<ICategoryStatisticService, CategoryStatisticService>();

			return services;
		}
	}
}
