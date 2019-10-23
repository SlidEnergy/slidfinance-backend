using SlidFinance.Domain;

namespace SlidFinance.App
{
	public class DataAccessLayer
    {
        public IRepositoryWithAccessCheck<Category> Categories { get; }

        public IRepositoryWithAccessCheck<Bank> Banks { get; }

		public IRepositoryWithAccessCheck<BankAccount> Accounts { get; }

		public IRepositoryWithAccessCheck<Rule> Rules { get; }

		public IRepositoryWithAccessCheck<Transaction> Transactions { get; }

		public IRepository<ApplicationUser, string> Users { get; }

		public IRefreshTokensRepository RefreshTokens { get; }

		public IRepository<Mcc, int> Mcc { get; }

		public DataAccessLayer(
            IRepositoryWithAccessCheck<Bank> banks, 
            IRepositoryWithAccessCheck<Category> categories,
            IRepository<ApplicationUser, string> users,
            IRepositoryWithAccessCheck<BankAccount> bankAccounts,
            IRepositoryWithAccessCheck<Rule> rules,
            IRepositoryWithAccessCheck<Transaction> transactions,
			IRefreshTokensRepository refreshTokens,
			IRepository<Mcc, int> mcc)
        {
            Banks = banks;
            Categories = categories;
            Users = users;
            Accounts = bankAccounts;
            Rules = rules;
            Transactions = transactions;
			RefreshTokens = refreshTokens;
			Mcc = mcc;
        }
    }
}
