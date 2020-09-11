using Microsoft.EntityFrameworkCore;
using Moq;
using SlidFinance.App;
using SlidFinance.Infrastructure;
using NUnit.Framework;
using System.Threading.Tasks;
using SlidFinance.Domain;
using Microsoft.AspNetCore.Identity;
using System;

namespace SlidFinance.WebApi.UnitTests
{
	public class TestsBase
	{
		protected readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();
		protected ApplicationDbContext _db;
		protected DataAccessLayer _mockedDal;
		protected ApplicationUser _user;

		protected Mock<IRepository<Bank, int>> _banks;
		protected Mock<IRepository<UserCategory, int>> _categories;
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

			_banks = new Mock<IRepository<Bank, int>>();
			_categories = new Mock<IRepository<UserCategory, int>>();
			_users = new Mock<IRepository<ApplicationUser, string>>();
			_accounts = new Mock<IRepository<BankAccount, int>>();
			_rules = new Mock<IRepository<Rule, int>>();
			_transactions = new Mock<IRepository<Transaction, int>>();
			_authTokens = new Mock<IAuthTokensRepository>();
			_mcc = new Mock<IRepository<Mcc, int>>();

			_mockedDal = new DataAccessLayer(_banks.Object, _categories.Object, _users.Object, _accounts.Object, _rules.Object, _transactions.Object, 
				_authTokens.Object, _mcc.Object);

			var role = new IdentityRole() { Name = Role.Admin };
			_db.Roles.Add(role);

			var trustee = new Trustee();
			_db.Trustee.Add(trustee);

			var userName = Guid.NewGuid() + "@mail.com";
			_user = new ApplicationUser() { Email = userName, UserName = userName, TrusteeId = trustee.Id, Trustee = trustee };
			_db.Users.Add(_user);
			
			_db.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = _user.Id });

			await _db.SaveChangesAsync();
		}
	}
}
