using Microsoft.EntityFrameworkCore;
using Moq;
using SlidFinance.App;
using SlidFinance.Infrastructure;
using NUnit.Framework;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class TestsBase
    {
        protected readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();
        protected ApplicationDbContext _db;
        protected DataAccessLayer _dal;
        protected DataAccessLayer _mockedDal;
        protected ApplicationUser _user;

        protected Mock<IRepository<Bank, int>> _banks;
        protected Mock<IRepository<Category, int>> _categories;
        protected Mock<IRepository<ApplicationUser, string>> _users;
        protected Mock<IRepository<BankAccount, int>> _accounts;
        protected Mock<IRepository<Rule, int>> _rules;
        protected Mock<IRepository<Transaction, int>> _transactions;
		protected Mock<IAuthTokensRepository> _authTokens;
		protected Mock<IRepository<Mcc, int>> _mcc;

		[SetUp]
        public async Task SetupBase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(TestContext.CurrentContext.Test.Name);
            _db = new ApplicationDbContext(optionsBuilder.Options);
            _dal = new DataAccessLayer(
                new EfRepository<Bank, int>(_db),
                new EfRepository<Category, int>(_db),
                new EfRepository<ApplicationUser, string>(_db),
                new EfRepository<BankAccount, int>(_db),
                new EfRepository<Rule, int>(_db),
                new EfRepository<Transaction, int>(_db),
				new EfAuthTokensRepository(_db),
				new EfRepository<Mcc, int>(_db));

            _banks = new Mock<IRepository<Bank, int>>();
            _categories = new Mock<IRepository<Category, int>>();
            _users = new Mock<IRepository<ApplicationUser, string>>();
            _accounts = new Mock<IRepository<BankAccount, int>>();
            _rules = new Mock<IRepository<Rule, int>>();
            _transactions = new Mock<IRepository<Transaction, int>>();
			_authTokens = new Mock<IAuthTokensRepository>();
			_mcc = new Mock<IRepository<Mcc, int>>();

			_mockedDal = new DataAccessLayer(_banks.Object, _categories.Object, _users.Object, _accounts.Object, _rules.Object, _transactions.Object, 
				_authTokens.Object, _mcc.Object);

            _user = await _dal.Users.Add(new ApplicationUser() { Email = "test1@email.com", Trustee = new Trustee() });
        }
    }
}
