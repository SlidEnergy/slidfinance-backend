using SlidFinance.Domain;

namespace SlidFinance.App
{
	public class DataAccessLayer
    {
        public IRepository<UserCategory, int> Categories { get; }

        public IRepository<Bank, int> Banks { get; }

		public IRepository<BankAccount, int> Accounts { get; }

		public IRepository<Rule, int> Rules { get; }

		public IRepository<Transaction, int> Transactions { get; }

		public IRepository<ApplicationUser, string> Users { get; }

		public IAuthTokensRepository AuthTokens { get; }

		public IRepository<Mcc, int> Mcc { get; }

		public DataAccessLayer(
            IRepository<Bank, int> banks, 
            IRepository<UserCategory, int> categories,
            IRepository<ApplicationUser, string> users,
            IRepository<BankAccount, int> bankAccounts,
            IRepository<Rule, int> rules,
            IRepository<Transaction, int> transactions,
			IAuthTokensRepository authTokens,
			IRepository<Mcc, int> mcc)
        {
            Banks = banks;
            Categories = categories;
            Users = users;
            Accounts = bankAccounts;
            Rules = rules;
            Transactions = transactions;
			AuthTokens = authTokens;
			Mcc = mcc;
        }
    }
}
