using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class ImportServiceTests : TestsBase
    {
        private ImportService _service;

        [SetUp]
        public void Setup()
        {
            _service = new ImportService(_mockedDal, _db);
        }

        [Test]
        public async Task ImportWithExistsRule_ShouldCallMethodWithRightCategory()
        {
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
            var rule = new Rule() { Category = category, Account = account };
			_db.Rules.Add(rule);

            var transaction = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

			await _db.SaveChangesAsync();

            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction });

            _transactions.Verify(x => x.Add(It.Is<Transaction>(t=>t.Category.Id == category.Id && t.Account.Id == account.Id)), Times.Exactly(1));
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task Import_ShouldCallAddMethodTwice()
        {
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
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

            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction1, transaction2 });

            _transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Exactly(2));
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task ImportDublicates_ShouldNotImported()
        {
            var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
            var category = _db.CreateCategory(_user);

            var transaction = new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

            _db.Transactions.Add(new Transaction() { DateTime = transaction.DateTime, Amount = transaction.Amount, Description = transaction.Description,
                Account = account });

			await _db.SaveChangesAsync();

            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());

            var count = await _service.Import(_user.Id, account.Code, 100, new Transaction[] { transaction });

            _transactions.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Never);
            Assert.AreEqual(0, count);
        }
    }
}