using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public static class EntityFactoryExtensions
	{
		public static async Task<BankAccount> CreateAccount(this ApplicationDbContext db, ApplicationUser user)
		{
			var account = new BankAccount()
			{
				Title = Guid.NewGuid().ToString(),
				Code = Guid.NewGuid().ToString()
			};
			db.Accounts.Add(account);
			db.TrusteeAccounts.Add(new TrusteeAccount(user, account));
			await db.SaveChangesAsync();

			return account;
		}

		public static async Task<Product> CreateProduct(this ApplicationDbContext db, ApplicationUser user, int bankId)
		{
			var product = new Product()
			{
				BankId = bankId,
				Title = Guid.NewGuid().ToString(),
			};
			db.Products.Add(product);
			db.TrusteeProducts.Add(new TrusteeProduct(user, product));
			await db.SaveChangesAsync();

			return product;
		}

		public static async Task<ProductTariff> CreateTariff(this ApplicationDbContext db, int productId)
		{
			var tariff = new ProductTariff()
			{
				ProductId = productId,
				Title = Guid.NewGuid().ToString(),
				Type = ProductType.Card
			};
			db.Tariffs.Add(tariff);
			await db.SaveChangesAsync();

			return tariff;
		}

		public static async Task<CashbackCategory> CreateCashbackCategory(this ApplicationDbContext db, int tariffId)
		{
			var model = new CashbackCategory()
			{
				TariffId = tariffId,
				Title = Guid.NewGuid().ToString(),
				Type = CashbackCategoryType.BaseCashback
			};
			db.CashbackCategories.Add(model);
			await db.SaveChangesAsync();

			return model;
		}

		public static async Task<CashbackCategoryMcc> CreateCashbackCategoryMcc(this ApplicationDbContext db, int categoryId, int mccCode)
		{
			var model = new CashbackCategoryMcc()
			{
				CategoryId = categoryId,
				MccCode = mccCode
			};
			db.CashbackCategoryMcc.Add(model);
			await db.SaveChangesAsync();

			return model;
		}

		public static async Task<Cashback> CreateCashback(this ApplicationDbContext db, int categoryId)
		{
			var model = new Cashback()
			{
				CategoryId = categoryId,
				Percent = 0.01f
			};
			db.Cashback.Add(model);
			await db.SaveChangesAsync();

			return model;
		}

		public static async Task<UserCategory> CreateCategory(this ApplicationDbContext db, ApplicationUser user)
		{
			var category = new UserCategory()
			{
				Title = Guid.NewGuid().ToString()
			};
			db.Categories.Add(category);
			await db.SaveChangesAsync();
			db.TrusteeCategories.Add(new TrusteeCategory(user, category));
			await db.SaveChangesAsync();

			return category;
		}

		public static async Task<ApplicationUser> CreateUser(this ApplicationDbContext db)
		{
			var trustee = new Trustee();
			db.Trustee.Add(trustee);
			await db.SaveChangesAsync();
			var user = new ApplicationUser() { Email = Guid.NewGuid().ToString(), TrusteeId = trustee.Id};
			db.Users.Add(user);
			await db.SaveChangesAsync();

			return user;
		}

		public static async Task<Transaction> CreateTransaction(this ApplicationDbContext db, BankAccount account)
		{
			var transaction = new Transaction()
			{
				Account = account
			};
			db.Transactions.Add(transaction);
			await db.SaveChangesAsync();

			return transaction;
		}

		public static async Task<Merchant> CreateMerchant(this ApplicationDbContext db, int mccId)
		{
			var merchant = new Merchant()
			{
				Name = Guid.NewGuid().ToString(),
				MccId = mccId
			};
			db.Merchants.Add(merchant);
			await db.SaveChangesAsync();

			return merchant;
		}

		public static async Task<Mcc> CreateMcc(this ApplicationDbContext db, string code)
		{
			var mcc = new Mcc() { Code = code };
			db.Mcc.Add(mcc);
			await db.SaveChangesAsync();

			return mcc;
		}

		public static async Task<Bank> CreateBank(this ApplicationDbContext db)
		{
			var bank = new Bank() { Title = Guid.NewGuid().ToString() };
			db.Banks.Add(bank);
			await db.SaveChangesAsync();

			return bank;
		}
	}
}
