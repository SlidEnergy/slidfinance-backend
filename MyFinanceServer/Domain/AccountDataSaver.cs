﻿using Microsoft.EntityFrameworkCore;
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

        public async Task Save(Models.Account account, float? accountBalance, ICollection<Models.Transaction> transactions)
        {
            if (account.Transactions == null)
                throw new ArgumentException(nameof(account));

            if(accountBalance != null && accountBalance.Value != 0)
                account.Balance = accountBalance.Value;

            foreach (var t in transactions)
            {
                var existTransaction = account.Transactions.FirstOrDefault(x => 
                    x.DateTime == t.DateTime && x.Amount == t.Amount && x.Description == t.Description);

                if(existTransaction == null)
                    account.Transactions.Add(t);
            }
            Console.Write("before save transactions");
            await _dbContext.SaveChangesAsync();
        }
    }
}
