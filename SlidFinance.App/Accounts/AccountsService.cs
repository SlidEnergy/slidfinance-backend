using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class AccountsService : IAccountsService
	{
		private IApplicationDbContext _context;

		public AccountsService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<BankAccount> GetByIdWithAccessCheckAsync(string userId, int id) => GetByIdWithChecksAsync(userId, id);

		public async Task<List<BankAccount>> GetListWithAccessCheckAsync(string userId, int? bankId = null)
		{
			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);

			if (bankId.HasValue)
				accounts = accounts.Where(x => x.Bank.Id == bankId).ToList();

			return accounts;
		}

		public async Task<BankAccount> AddAccount(string userId, BankAccount account)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			var bank = await _context.Banks.FindAsync(account.BankId);

			if (bank == null)
				throw new EntityNotFoundException();

			_context.Accounts.Add(account);
			await _context.SaveChangesAsync();

			_context.TrusteeAccounts.Add(new TrusteeAccount(user, account));
			await _context.SaveChangesAsync();

			return account;
		}

		public async Task<BankAccount> Update(string userId, BankAccount account)
		{
			var model = await GetByIdWithChecksAsync(userId, account.Id);

			model.Title = account.Title;
			model.Code = account.Code;
			model.Balance = account.Balance;
			model.CreditLimit = account.CreditLimit;
			model.SelectedTariffId = account.SelectedTariffId;
			model.ProductId = account.ProductId;
			model.Opened = account.Opened;
			model.SaltedgeBankAccountId = account.SaltedgeBankAccountId;

			_context.Accounts.Update(model);
			await _context.SaveChangesAsync();

			return model;
		}

		public async Task<BankAccount> PatchAccount(string userId, BankAccount account)
		{
			var user = await _context.Users.FindAsync(userId);

			Bank bank = null;

			if (account.Bank!= null)
			{
				bank = await _context.Banks.FindAsync(account.Bank.Id);

				if (bank == null)
					throw new EntityNotFoundException();
			}

			_context.Accounts.Update(account);
			await _context.SaveChangesAsync();

			return account;
		}

		public async Task DeleteAccount(string userId, int accountId)
		{
			var account = await GetByIdWithChecksAsync(userId, accountId);

			_context.Accounts.Remove(account);
			await _context.SaveChangesAsync();
		}

		private async Task<BankAccount> GetByIdWithChecksAsync(string userId, int id)
		{
			var account = await _context.Accounts.FindAsync(id);

			if (account == null)
				throw new EntityNotFoundException();

			await CheckAccessAndThrowException(userId, account);

			return account;
		}

		private async Task CheckAccessAndThrowException(string userId, BankAccount account)
		{
			var user = await _context.Users.FindAsync(userId);

			await CheckAccessAndThrowException(user, account);
		}

		private async Task CheckAccessAndThrowException(ApplicationUser user, BankAccount account)
		{
			var trustee = await _context.TrusteeAccounts
				.Where(t => t.TrusteeId == user.TrusteeId)
				.Join(_context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a)
				.Where(a => a.Id == account.Id)
				.FirstOrDefaultAsync();

			if (trustee == null)
				throw new EntityAccessDeniedException();
		}
	}
}
