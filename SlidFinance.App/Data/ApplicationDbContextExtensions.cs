using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public static class ApplicationDbContextExtensions
	{
		public static async Task<List<CashbackCategoryMcc>> GetCashbackCategoryMccWithAccessCheckAsync(this IApplicationDbContext context, string userId, int categoryId)
		{
			var user = await context.Users.FindAsync(userId);

			var category = await context.GetCashbackCategoryByIdWithAccessCheck(userId, categoryId);

			if (category == null)
				return null;

			return await context.CashbackCategoryMcc.Where(x => x.CategoryId == categoryId).ToListAsync();
		}

		public static async Task<List<CashbackCategory>> GetCashbackCategoriesWithAccessCheckAsync(this IApplicationDbContext context, string userId, int? tariffId = null)
		{
			var user = await context.Users.FindAsync(userId);

			if (tariffId.HasValue)
			{
				var tariff = await context.GetProductTariffByIdWithAccessCheck(userId, tariffId.Value);

				if (tariff == null)
					return null;

				return await context.CashbackCategories.Where(x => x.TariffId == tariffId).ToListAsync();
			}
			else
			{
				return await context.TrusteeAccounts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a)
				.Join(context.CashbackCategories, acc => acc.SelectedTariffId, cat => cat.TariffId, (acc, cat) => cat)
				.ToListAsync();
			}
		}

		public static async Task<List<ProductTariff>> GetProductTariffsWithAccessCheckAsync(this IApplicationDbContext context, string userId, int productId)
		{
			var user = await context.Users.FindAsync(userId);

			var product = await context.GetProductByIdWithAccessCheck(userId, productId);

			if (product == null)
				return null;

			return await context.Tariffs.Where(x => x.ProductId == productId).ToListAsync();
		}

		public static async Task<List<Product>> GetProductListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			var allProducts = await context.Products.Where(x => x.IsPublic && x.Approved).ToListAsync();

			var products = await context.TrusteeProducts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Products, t => t.ProductId, p => p.Id, (t, p) => p)
				.ToListAsync();

			return allProducts.Union(products).ToList();
		}

		public static async Task<List<BankAccount>> GetAccountListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			var accounts = await context.TrusteeAccounts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a)
				.ToListAsync();

			return accounts;
		}

		public static async Task<List<Transaction>> GetTransactionListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			var transactions = await context.TrusteeAccounts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Transactions, a => a.AccountId, t => t.AccountId, (a, t) => t)
				.ToListAsync();

			return transactions;
		}

		public static async Task<List<UserCategory>> GetCategoryListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			var categories = await context.TrusteeCategories
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Categories, t => t.CategoryId, c => c.Id, (t, c) => c)
				.ToListAsync();

			return categories;
		}

		public static async Task<List<Rule>> GetRuleListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			var rules = await context.TrusteeCategories
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Rules, c => c.CategoryId, r => r.CategoryId, (c, r) => r)
				.ToListAsync();

			return rules;
		}

		public static async Task<CashbackCategory> GetCashbackCategoryByIdWithAccessCheck(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var category = await context.CashbackCategories.FindAsync(id);

			if (category == null)
				return null;

			var tariff = await context.Tariffs.FindAsync(category.TariffId);

			if (tariff == null)
				return null;

			var product = await context.Products.FindAsync(tariff.ProductId);

			if (product == null)
				return null;

			if (product.IsPublic && product.Approved)
				return category;

			if (await context.TrusteeProducts
				.Where(t => t.TrusteeId == user.TrusteeId && t.ProductId == tariff.ProductId).FirstOrDefaultAsync() != null)
				return category;

			return null;
		}

		public static async Task<ProductTariff> GetProductTariffByIdWithAccessCheck(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var tariff = await context.Tariffs.FindAsync(id);

			if (tariff == null)
				return null;

			var product = await context.Products.FindAsync(tariff.ProductId);

			if (product == null)
				return null;

			if (product.IsPublic && product.Approved)
				return tariff;

			if (await context.TrusteeProducts
				.Where(t => t.TrusteeId == user.TrusteeId && t.ProductId == tariff.ProductId).FirstOrDefaultAsync() != null)
				return tariff;

			return null;
		}

		public static async Task<Product> GetProductByIdWithAccessCheck(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var product = await context.Products.FindAsync(id);

			if (product == null)
				return null;

			if (product.IsPublic && product.Approved)
				return product;

			if (await context.TrusteeProducts
				.Where(t => t.TrusteeId == user.TrusteeId && t.ProductId == id).FirstOrDefaultAsync() != null)
				return product;

			return null;
		}

		public static async Task<BankAccount> GetAccountByIdWithAccessCheck(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			return await context.TrusteeAccounts
				.Where(t => t.TrusteeId == user.TrusteeId)
				.Join(context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a)
				.Where(a => a.Id == id)
				.FirstOrDefaultAsync();
		}

		public static async Task<Transaction> GetTransactionByIdWithAccessCheckAsync(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var transactions = await context.TrusteeAccounts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Transactions, a => a.AccountId, t => t.AccountId, (a, t) => t)
				.Where(t => t.Id == id)
				.FirstOrDefaultAsync();

			return transactions;
		}

		public static async Task<UserCategory> GetCategorByIdWithAccessCheckAsync(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var categories = await context.TrusteeCategories
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Categories, t => t.CategoryId, c => c.Id, (t, c) => c)
				.Where(c => c.Id == id)
				.FirstOrDefaultAsync();

			return categories;
		}

		public static async Task<Rule> GetRuleByIdWithAccessCheckAsync(this IApplicationDbContext context, string userId, int id)
		{
			var user = await context.Users.FindAsync(userId);

			var rules = await context.TrusteeCategories
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(context.Rules, c => c.CategoryId, r => r.CategoryId, (c, r) => r)
				.Where(r => r.Id == id)
				.FirstOrDefaultAsync();

			return rules;
		}

		public static async Task<List<Merchant>> GetMerchantListWithAccessCheckAsync(this IApplicationDbContext context, string userId)
		{
			var isAdmin = await context.IsAdmin(userId);
			if (isAdmin)
				return await context.Merchants.ToListAsync();

			return await context.Merchants.Where(x => x.IsPublic == true).ToListAsync();
		}

		public static async Task<bool> IsAdmin(this IApplicationDbContext context, string userId)
		{
			var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == Role.Admin);

			if (role == null)
				throw new Exception("Роль администратора не найдена.");

			var userRole = await context.UserRoles.FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == userId);

			return userRole == null ? false : true;
		}

		public static async Task<Merchant> GetMerchantByIdWithAccessCheckAsync(this IApplicationDbContext context, string userId, int id)
		{
			var isAdmin = await context.IsAdmin(userId);
			if (isAdmin)
				return await context.Merchants.FindAsync(id);

			return await context.Merchants.FindAsync(id);
		}

		public static async Task<SaltedgeAccount> GetSaltedgeAccountByIdWithAccessCheck(this IApplicationDbContext context, string userId)
		{
			var user = await context.Users.FindAsync(userId);

			return await context.TrusteeSaltedgeAccounts
				.Where(t => t.TrusteeId == user.TrusteeId)
				.Join(context.SaltedgeAccounts, t => t.SaltedgeAccountId, a => a.Id, (t, a) => a)
				.FirstOrDefaultAsync();
		}
	}
}
