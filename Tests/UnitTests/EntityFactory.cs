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

		public static async Task<Category> CreateCategory(this ApplicationDbContext db, ApplicationUser user)
		{
			var category = new Category()
			{
				Title = Guid.NewGuid().ToString()
			};
			db.Categories.Add(category);
			db.TrusteeCategories.Add(new TrusteeCategory(user, category));
			await db.SaveChangesAsync();

			return category;
		}

		public static async Task<ApplicationUser> CreateUser(this ApplicationDbContext db)
		{
			var user = new ApplicationUser() { Email = Guid.NewGuid().ToString() };
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
	}
}
