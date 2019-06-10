using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinanceServer.Core;

namespace MyFinanceServer.Core
{
    public class DataAccessLayer
    {
        public IRepositoryWithAccessCheck<Category> Categories;

        public IRepositoryWithAccessCheck<Bank> Banks;

        public IRepositoryWithAccessCheck<BankAccount> Accounts;

        public IRepositoryWithAccessCheck<Rule> Rules;

        public IRepositoryWithAccessCheck<Transaction> Transactions;

        public IRepository<ApplicationUser, string> Users;

		public IRefreshTokensRepository RefreshTokens;

		public DataAccessLayer(
            IRepositoryWithAccessCheck<Bank> banks, 
            IRepositoryWithAccessCheck<Category> categories,
            IRepository<ApplicationUser, string> users,
            IRepositoryWithAccessCheck<BankAccount> bankAccounts,
            IRepositoryWithAccessCheck<Rule> rules,
            IRepositoryWithAccessCheck<Transaction> transactions,
			IRefreshTokensRepository refreshTokens)
        {
            Banks = banks;
            Categories = categories;
            Users = users;
            Accounts = bankAccounts;
            Rules = rules;
            Transactions = transactions;
			RefreshTokens = refreshTokens;
        }
    }
}
