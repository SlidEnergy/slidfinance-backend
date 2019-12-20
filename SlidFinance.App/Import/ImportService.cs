using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class ImportService : IImportService
	{
		private IApplicationDbContext _context;

		public ImportService(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<int> Import(string userId, string accountCode, float? balance, Transaction[] transactions)
		{
			var account = await GetAccount(userId, accountCode);

			await UpdateBalance(account, balance);

			var rules = await _context.GetRuleListWithAccessCheckAsync(userId);

			var count = 0;

			foreach (var t in transactions)
			{
				var existTransaction = account.Transactions.Any(x => x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description);

				if (!existTransaction)
				{
					t.Account = account;
					t.Category = GetCategoryByRules(t, rules);
					_context.Transactions.Add(t);
					await _context.SaveChangesAsync();
					count++;
				}
			}

			return count;
		}

		private async Task UpdateBalance(BankAccount account, float? balance)
		{
			if (balance.HasValue && balance.Value != 0)
			{
				account.Balance = balance.Value;
				_context.Accounts.Update(account);
				await _context.SaveChangesAsync();
			}
		}

		private Category GetCategoryByRules(Transaction t, ICollection<Rule> rules)
		{
			var rule = rules.FirstOrDefault(x =>
				(x.Account == null || x.Account.Id.Equals(t.Account.Id)) &&
				(string.IsNullOrEmpty(x.BankCategory) || x.BankCategory.Equals(t.BankCategory)) &&
				(string.IsNullOrEmpty(x.Description) || x.Description.Equals(t.Description)) &&
				(x.Mcc == null || x.Mcc.Equals(t.Mcc)));

			return rule?.Category;
		}

		private async Task<BankAccount> GetAccount(string userId, string accountCode)
		{
			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);

			var account = accounts.FirstOrDefault(x => x.Code == accountCode);

			if (account == null)
				throw new EntityNotFoundException();

			return account;
		}

		private async Task<Models.Merchant> AddMerchantIfNotExist(Transaction t)
		{
			var merchant = new Models.Merchant() { Name = t.Description };

			_context.Merchants.Add(merchant);
			await _context.SaveChangesAsync();
			return merchant;
		}
	}
}
