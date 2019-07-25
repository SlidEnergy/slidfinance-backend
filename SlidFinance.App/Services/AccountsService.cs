using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class AccountsService
    {
        private DataAccessLayer _dal;

        public AccountsService(DataAccessLayer dal)
        {
            _dal = dal;
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
