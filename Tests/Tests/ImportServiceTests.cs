using Moq;
using MyFinanceServer.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class ImportServiceTests : TestsBase
    {
        private ImportService _service;

        [SetUp]
        public void Setup()
        {
            _service = new ImportService(_mockedDal);
        }

        [Test]
        public async Task ImportWithExistsRule_ShouldCallMethodWithRightCategory()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var rule = await _dal.Rules.Add(new Rule() { Category = category, Account = account });

            var transaction = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<BankAccount>() { account });
            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());
            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Rule>() { rule });

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction });

            _transactions.Verify(x => x.Add(It.Is<Transaction>(t=>t.Category.Id == category.Id && t.Account.Id == account.Id)), Times.Exactly(1));
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task Import_ShouldCallAddMethodTwice()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var transaction1 = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };
            var transaction2 = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Description = "Description #2",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<BankAccount>() { account });
            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());
            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Rule>());

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction1, transaction2 });

            _transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Exactly(2));
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task ImportDublicates_ShouldNotImported()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });

            var transaction = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

            await _dal.Transactions.Add(new Transaction() { DateTime = transaction.DateTime, Amount = transaction.Amount, Description = transaction.Description,
                Account = account });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<BankAccount>() { account });
            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());
            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Rule>());

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction });

            _transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Never);
            Assert.AreEqual(0, count);
        }
    }
}