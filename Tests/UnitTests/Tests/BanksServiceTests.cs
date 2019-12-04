using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class BanksServiceTests : TestsBase
    {
        private BanksService _service;

        [SetUp]
        public void Setup()
        {
            _service = new BanksService(_mockedDal, _db);
        }

        [Test]
        public async Task AddBank_ShouldCallAddMethodWithRightArguments()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.Add(It.IsAny<Bank>())).ReturnsAsync(new Bank());

            var category1 = await _service.AddBank(_user.Id, "Bank #1");

            _banks.Verify(x => x.Add(
                It.Is<Bank>(c => c.Title == "Bank #1" && c.User.Id == _user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task DeleteBank_ShouldCallAddMethodWithRightArguments()
        {
            var bank = await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = _user
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(bank);
            _banks.Setup(x => x.Delete(It.IsAny<Bank>())).Returns(Task.CompletedTask);

            await _service.DeleteBank(_user.Id, bank.Id);

            _banks.Verify(x => x.Delete(
                It.Is<Bank>(c => c.Title == bank.Title && bank.User.Id == bank.User.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetBanks_ShouldReturnList()
        {
			var bank1 = new Bank()
			{
				Title = "Bank #1",
				User = _user
			};
			_db.Banks.Add(bank1);
			var bank2 = new Bank()
			{
				Title = "Bank #2",
				User = _user
			};
			_db.Banks.Add(bank2);
			var account1 = new BankAccount()
			{
				Title = "Account #1",
				Bank = bank1
			};
			_db.Accounts.Add(account1);
			var account2 = new BankAccount()
			{
				Title = "Account #2",
				Bank = bank2
			};
			_db.Accounts.Add(account2);
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account1.Id });
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account2.Id });
			await _db.SaveChangesAsync();

            var result = await _service.GetListWithAccessCheckAsync(_user.Id);
            
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetBanks_ShouldBeCorrectBalance()
        {
			
			var bank1 = new Bank()
			{
				Title = "Bank #1",
				User = _user
			};
			var account4 = new BankAccount { Title = "Account #1", Balance = 0, Bank = bank1 };
			_db.Accounts.Add(account4);
			var bank2 = new Bank()
			{
				Title = "Bank #2",
				User = _user
			};
			var account1 = new BankAccount { Title = "Account #1", Balance = 100, Bank = bank2 };
			var account2 = new BankAccount { Title = "Account #2", Balance = 200, Bank = bank2 };
			var account3 = new BankAccount { Title = "Account #3", Balance = 300, Bank = bank2 };
			_db.Accounts.Add(account1);
			_db.Accounts.Add(account2);
			_db.Accounts.Add(account3);
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account1.Id });
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account2.Id });
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account3.Id });
			_db.TrusteeAccounts.Add(new TrusteeAccount() { TrusteeId = _user.TrusteeId, AccountId = account4.Id });
			await _db.SaveChangesAsync();

            var result = await _service.GetListWithAccessCheckAsync(_user.Id);

            Assert.AreEqual(0, result[0].OwnFunds);
            Assert.AreEqual(600, result[1].OwnFunds);
        }
    }
}