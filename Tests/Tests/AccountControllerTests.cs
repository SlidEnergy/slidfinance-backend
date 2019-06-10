using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
	public class AccountControllerTests : TestsBase
    {
		private AccountsController _controller;

		[SetUp]
        public void Setup()
        {
            var service = new AccountsService(_mockedDal);
			_controller = new AccountsController(_autoMapper.Create(_db), service);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetAccounts_ShouldReturnList()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #2", Bank = bank });

            _accounts.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(bank.Accounts.ToList());

            var result = await _controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}