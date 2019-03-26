using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class AccountTests : TestsBase
    {
        private AccountsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new AccountsService(_dal);
        }

        [Test]
        public async Task GetAccounts_ShouldReturnList()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #2", Bank = bank });

            var accountDataSaver = new AccountDataSaver(_db);
            var controller = new AccountsController(_db, accountDataSaver, _autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }


        [Test]
        public async Task PatchAccountData_NoContentResult()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code ="code_1", Transactions = new List<Transaction>(), Bank = bank });

            var transaction1 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var transaction2 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = "Category #2",
                Description = "Description #2"
            };

            var accountDataSaver = new AccountDataSaver(_db);
            var controller = new AccountsController(_db, accountDataSaver, _autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.PatchAccountData(account.Code,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            var newAccount = await _db.Accounts.Include(x => x.Transactions).SingleOrDefaultAsync(x => x.Id == account.Id);
            Assert.AreEqual(500, result.Value.Balance);
            Assert.AreEqual(2, newAccount.Transactions.Count);
        }

        [Test]
        public async Task PatchAccountData_ShouldCallSaveMethod()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Transactions = new List<Transaction>(), Bank = bank, Code = "Code #1"});

            var transaction1 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var transaction2 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = "Category #2",
                Description = "Description #2"
            };

            var accountDataSaverMock = new Mock<IAccountDataSaver>();
            accountDataSaverMock
                .Setup(x => 
                    x.Save(It.IsAny<string>(), It.IsAny<BankAccount>(), It.IsAny<float>(), It.IsAny<ICollection<Transaction>>()))
                .Returns(Task.CompletedTask);

            var controller = new AccountsController(_db, accountDataSaverMock.Object, _autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.PatchAccountData(account.Code,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            accountDataSaverMock.Verify(x=> 
                x.Save(It.IsAny<string>(), It.IsAny<BankAccount>(), It.IsAny<float>(), It.IsAny<ICollection<Transaction>>()),
                Times.Once);
        }
    }
}