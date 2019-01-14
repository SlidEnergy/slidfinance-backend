using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Domain
{
    public class AccountDataSaver : IAccountDataSaver
    {
        public readonly ApplicationDbContext _dbContext;

        public AccountDataSaver()
        {
        }


        public AccountDataSaver(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Save(Models.Account account, float accountBalance, ICollection<Models.Transaction> transactions)
        {
            if (account.Transactions == null)
                throw new ArgumentException(nameof(account));

            account.Balance = accountBalance;

            foreach (var t in transactions)
            {
                var existTransaction = account.Transactions.Where(x => x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description).SingleOrDefault();

                if(existTransaction == null)
                    account.Transactions.Add(t);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
