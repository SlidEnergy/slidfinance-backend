using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Collections.Generic;

namespace SlidFinance.WebApi.UnitTests
{
	public class AccountControllerTests : TestsBase
    {
		private AccountsController _controller;
		private Mock<IAccountsService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<IAccountsService>();
			_controller = new AccountsController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetAccounts_ShouldReturnList()
        {
            var bank = new Bank() { Title = "Bank #1", User = _user };
            var account1 = new BankAccount() { Title = "Account #1", Bank = bank };
            var account2 = new BankAccount() { Title = "Account #2", Bank = bank };

			_service.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(new List<BankAccount>() { account1, account2 });

            var result = await _controller.GetList();

            Assert.AreEqual(2, result.Value.Count());

			_service.Verify(x => x.GetListWithAccessCheck(It.Is<string>(u => u == _user.Id), It.Is<int?>(b => b == null)));
		}
    }
}