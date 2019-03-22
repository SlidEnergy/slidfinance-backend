using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class BanksServiceTests : TestsBase
    {
        private Mock<IBanksRepository> _repository;
        private BanksService _service;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IBanksRepository>();
            _service = new BanksService(_repository.Object);
        }

        [Test]
        public async Task AddBank_ShouldCallAddMethodWithRightArguments()
        {
            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.Add(It.IsAny<Bank>())).ReturnsAsync(new Bank());

            var category1 = await _service.AddBank(_user.Id, "Bank #1");

            _repository.Verify(x => x.Add<Bank>(
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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.GetById<int, Bank>(It.IsAny<int>())).ReturnsAsync(bank);
            _repository.Setup(x => x.Delete(It.IsAny<Bank>())).Returns(Task.CompletedTask);

            await _service.DeleteBank(_user.Id, bank.Id);

            _repository.Verify(x => x.Delete(
                It.Is<Bank>(c => c.Title == bank.Title && bank.User.Id == bank.User.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetBanks_ShouldCallGetListMethodWithRightArguments()
        {
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _service.GetList(_user.Id);

            _repository.Verify(x => x.GetListWithAccessCheck(
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

            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

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

            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _service.GetList(_user.Id);

            Assert.AreEqual(0, result[0].OwnFunds);
            Assert.AreEqual(600, result[1].OwnFunds);
        }
    }
}