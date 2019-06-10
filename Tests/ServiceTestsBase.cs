using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TestsBase
    {
        protected readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();
        protected ApplicationDbContext _db;
        protected DataAccessLayer _dal;
        protected DataAccessLayer _mockedDal;
        protected ApplicationUser _user;

        protected Mock<IRepositoryWithAccessCheck<Bank>> _banks;
        protected Mock<IRepositoryWithAccessCheck<Category>> _categories;
        protected Mock<IRepository<ApplicationUser, string>> _users;
        protected Mock<IRepositoryWithAccessCheck<BankAccount>> _accounts;
        protected Mock<IRepositoryWithAccessCheck<Rule>> _rules;
        protected Mock<IRepositoryWithAccessCheck<Transaction>> _transactions;
		protected Mock<IRefreshTokensRepository> _refreshTokens;

        [SetUp]
        public async Task SetupBase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(TestContext.CurrentContext.Test.Name);
            _db = new ApplicationDbContext(optionsBuilder.Options);
            _dal = new DataAccessLayer(
                new EfBanksRepository(_db),
                new EfCategoriesRepository(_db),
                new EfRepository<ApplicationUser, string>(_db),
                new EfBankAccountsRepository(_db),
                new EfRulesRepository(_db),
                new EfTransactionsRepository(_db),
				new EfRefreshTokensRepository(_db));

            _banks = new Mock<IRepositoryWithAccessCheck<Bank>>();
            _categories = new Mock<IRepositoryWithAccessCheck<Category>>();
            _users = new Mock<IRepository<ApplicationUser, string>>();
            _accounts = new Mock<IRepositoryWithAccessCheck<BankAccount>>();
            _rules = new Mock<IRepositoryWithAccessCheck<Rule>>();
            _transactions = new Mock<IRepositoryWithAccessCheck<Transaction>>();
			_refreshTokens = new Mock<IRefreshTokensRepository>();

			_mockedDal = new DataAccessLayer(_banks.Object, _categories.Object, _users.Object, _accounts.Object, _rules.Object, _transactions.Object, 
				_refreshTokens.Object);

            _user = await _dal.Users.Add(new ApplicationUser() { Email = "test1@email.com" });
        }
    }
}
