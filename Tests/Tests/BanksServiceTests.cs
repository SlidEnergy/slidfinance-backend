using Moq;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class BanksServiceTests : TestsBase
    {
        private BanksService _service;

        [SetUp]
        public void Setup()
        {
            _service = new BanksService(_mockedDal);
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
        public async Task GetBanks_ShouldCallGetListMethodWithRightArguments()
        {
            _banks.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _service.GetList(_user.Id);

            _banks.Verify(x => x.GetListWithAccessCheck(
                    It.Is<string>(c => c == _user.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetBanks_ShouldReturnList()
        {
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = _user
            });
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = _user
            });

            _banks.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _service.GetList(_user.Id);
            
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetBanks_ShouldBeCorrectBalance()
        {
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = _user
            });
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = _user,
                Accounts = new List<BankAccount>() {
                    new BankAccount { Title = "Account #1", Balance = 100},
                    new BankAccount { Title = "Account #2", Balance = 200},
                    new BankAccount { Title = "Account #3", Balance = 300},
                }
            });

            _banks.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _service.GetList(_user.Id);

            Assert.AreEqual(0, result[0].OwnFunds);
            Assert.AreEqual(600, result[1].OwnFunds);
        }
    }
}