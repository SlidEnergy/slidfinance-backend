using MyFinanceServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Domain
{
    public class AccountDataSaver : IAccountDataSaver
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountDataSaver(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Save(BankAccount account, float? accountBalance, ICollection<Transaction> transactions)
        {
            // TODO: метод должен сам запрашивать список транзакций, если они не заданы

            if (account.Transactions == null)
                throw new ArgumentException(nameof(account));

            if (accountBalance != null && accountBalance.Value != 0)
                account.Balance = accountBalance.Value;

            foreach (var t in transactions)
            {
                var existTransaction = account.Transactions.FirstOrDefault(x => 
                    x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description);

                if(existTransaction == null)
                    account.Transactions.Add(t);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
