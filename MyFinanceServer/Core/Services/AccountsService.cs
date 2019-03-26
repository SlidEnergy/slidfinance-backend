using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class AccountsService
    {
        private DataAccessLayer _dal;

        public AccountsService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<List<BankAccount>> GetList(string userId, int? bankId)
        {
            var accounts = await _dal.Accounts.GetListWithAccessCheck(userId);

            return accounts.Where(x => bankId == null || x.Bank.Id == bankId).ToList();
        }

        public async Task<BankAccount> AddAccount(string userId, int bankId, string title, string code, float balance, float creditLimit)
        {
            var user = await _dal.Users.GetById(userId);

            var bank = await _dal.Banks.GetById(bankId);

            if (!bank.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            var account = await _dal.Accounts.Add(new BankAccount(bank, title, code, balance, creditLimit));

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

        public async Task DeleteAccount(string userId, int accountId)
        {
            var user = await _dal.Users.GetById(userId);

            var account = await _dal.Accounts.GetById(accountId);

            account.IsBelongsTo(userId);

            await _dal.Accounts.Delete(account);
        }
    }
}
