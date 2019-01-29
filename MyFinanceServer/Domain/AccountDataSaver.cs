using MyFinanceServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyFinanceServer.Domain
{
    public class AccountDataSaver : IAccountDataSaver
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountDataSaver(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Save(string userId, BankAccount account, float? accountBalance, ICollection<Transaction> transactions)
        {
            // TODO: метод должен сам запрашивать список транзакций, если они не заданы

            if (account.Transactions == null)
                throw new ArgumentException(nameof(account));

            if (accountBalance != null && accountBalance.Value != 0)
                account.Balance = accountBalance.Value;

            var rules = await _dbContext.Rules
                .Include(x=>x.Account)
                .Include(x=>x.Category)
                .Where(x => x.Category.User.Id == userId)
                .ToListAsync();

            foreach (var t in transactions)
            {
                var existTransaction = account.Transactions.FirstOrDefault(x => 
                    x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description);

                if (existTransaction == null)
                {
                    t.Category = GetCategoryByRules(t, rules);
                    account.Transactions.Add(t);
                }
            }
            await _dbContext.SaveChangesAsync();
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
    }
}
