using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class ImportService
    {
        private DataAccessLayer _dal;

        public ImportService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<int> Import(string userId, string accountCode, float? balance, Transaction[] transactions)
        {
            var user = await _dal.Users.GetById(userId);
            var account = await GetAccount(userId, accountCode);

            if (balance.HasValue && balance.Value != 0)
            {
                account.Balance = balance.Value;
                await _dal.Accounts.Update(account);
            }

            var rules = await _dal.Rules.GetListWithAccessCheck(userId);

            var count = 0;

            foreach (var t in transactions)
            {
                var existTransaction = account.Transactions.Any(x => x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description);

                if (!existTransaction)
                {
                    t.Account = account;
                    t.Category = GetCategoryByRules(t, rules);
                    await _dal.Transactions.Add(t);
                    count++;
                }
            }

            return count;
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
            var accounts = await _dal.Accounts.GetListWithAccessCheck(userId);

            var account = accounts.FirstOrDefault(x => x.Code == accountCode);

            if (account == null)
                throw new EntityNotFoundException();

            return account;
        }
    }
}
