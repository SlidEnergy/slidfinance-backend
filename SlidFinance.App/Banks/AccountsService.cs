using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class AccountsService : IAccountsService
	{
        private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public AccountsService(DataAccessLayer dal, IApplicationDbContext context)
        {
            _dal = dal;
			_context = context;
		}

		public Task<BankAccount> GetByIdWithAccessCheck(string userId, int id) => GetByIdWithChecks(userId, id);

        public async Task<List<BankAccount>> GetListWithAccessCheckAsync(string userId, int? bankId = null)
        {
			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);

			if (bankId.HasValue)
				accounts = accounts.Where(x => x.Bank.Id == bankId).ToList();

            return accounts;
        }

        public async Task<BankAccount> AddAccount(string userId, int bankId, string title, string code, float balance, float creditLimit)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var bank = await _context.Banks.FindAsync(bankId);

			var account = new BankAccount(bank, title, code, balance, creditLimit);
			_context.Accounts.Add(account);
			_context.TrusteeAccounts.Add(new TrusteeAccount(user, account));
			await _context.SaveChangesAsync();

            return account;
        }

        public async Task<BankAccount> EditAccount(string userId, int accountId, string title, string code, float balance, float creditLimit)
        {
            var editAccount = await GetByIdWithChecks(userId, accountId);

			editAccount.Update(title, code, balance, creditLimit);

            await _dal.Accounts.Update(editAccount);

            return editAccount;
        }

        public async Task<BankAccount> PatchAccount(string userId, BankAccount account)
        {
            var user = await _dal.Users.GetById(userId);

            Bank bank = null;

            if (account.Bank!= null)
            {
                bank = await _dal.Banks.GetById(account.Bank.Id);

                if (bank == null)
                    throw new EntityNotFoundException();
            }

            var newAccount = await _dal.Accounts.Update(account);

            return newAccount;
        }

        public async Task DeleteAccount(string userId, int accountId)
        {
			var account = await GetByIdWithChecks(userId, accountId);

			await _dal.Accounts.Delete(account);
        }

		private async Task<BankAccount> GetByIdWithChecks(string userId, int id)
		{
			var account = await _dal.Accounts.GetById(id);

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
