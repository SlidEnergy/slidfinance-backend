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

        public async Task<BankAccount> GetById(string userId, int id)
        {
            var account = await _dal.Accounts.GetById(id);

            if (account == null)
                throw new EntityNotFoundException();

            if (!account.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            return account;
        }

        public async Task<List<BankAccount>> GetListWithAccessCheck(string userId, int? bankId)
        {
			var user = await _context.Users.FindAsync(userId);

			var accounts = await _context.TrusteeAccounts.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(_context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a).ToListAsync();

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
			_context.TrusteeAccounts.Add(new TrusteeAccount() { Account = account, TrusteeId = user.TrusteeId });
			await _context.SaveChangesAsync();

            return account;
        }

        public async Task<BankAccount> EditAccount(string userId, int accountId, string title, string code, float balance, float creditLimit)
        {
            var editAccount = await _dal.Accounts.GetById(accountId);

            if (!editAccount.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

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

                if (!bank.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            var newAccount = await _dal.Accounts.Update(account);

            return newAccount;
        }

        public async Task DeleteAccount(string userId, int accountId)
        {
            var user = await _dal.Users.GetById(userId);

            var account = await _dal.Accounts.GetById(accountId);

            account.IsBelongsTo(userId);

            await _dal.Accounts.Delete(account);
        }
    }
}
