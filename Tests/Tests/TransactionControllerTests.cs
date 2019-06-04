using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TransactionControllerTests : TestsBase
    {
        private TransactionsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new TransactionsService(_mockedDal);
        }

        [Test]
        public async Task GetTransactions_ShouldReturnList()
        {
            var bank = await _dal.Banks.Add(new Bank() {Title = "Bank #1", User = _user});
            var account = await _dal.Accounts.Add(new BankAccount() { Transactions = new List<Transaction>(), Bank = bank });
            await _dal.Transactions.Add(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                Account = account
            });
            await _dal.Transactions.Add(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Description = "Description #2",
                Account = account
            });

            _transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(await _dal.Transactions.GetListWithAccessCheck(_user.Id));

            var controller = new TransactionsController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

		[Test]
		public async Task GetTransactionsForCategoryAndPeriod_ShouldReturnFilteredList()
		{
			var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
			var account = await _dal.Accounts.Add(new BankAccount() { Transactions = new List<Transaction>(), Bank = bank });
			var category1 = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
			var category2 = await _dal.Categories.Add(new Category() { Title = "Category #2", User = _user });
			await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 1),
				Amount = 10,
				Description = "Description #1",
				Account = account,
				Category = category1
			});
			var transaction = await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 2),
				Amount = 5,
				Description = "Description #2",
				Account = account,
				Category = category1
			});
			await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 3),
				Amount = 10,
				Description = "Description #1",
				Account = account,
				Category = category2
			});
			await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 4),
				Amount = 5,
				Description = "Description #2",
				Account = account,
				Category = null
			});
			await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 5),
				Amount = 5,
				Description = "Description #2",
				Account = account,
				Category = category1
			});

			_transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(await _dal.Transactions.GetListWithAccessCheck(_user.Id));

			var controller = new TransactionsController(_autoMapper.Create(_db), _service);
			controller.AddControllerContext(_user);
			var result = await controller.GetList(category1.Id, new DateTime(2019, 6, 2), new DateTime(2019, 6, 4));

			Assert.AreEqual(1, result.Value.Count());
			Assert.AreEqual(transaction.Id, result.Value.ToArray()[0].Id);
		}

		[Test]
		public void GetTransactionsWithInvalidArguments_ShouldThrowArgumentOutOfRangeException()
		{
			var controller = new TransactionsController(_autoMapper.Create(_db), _service);
			controller.AddControllerContext(_user);

			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.GetList(-1));
		}

		[Test]
		public void GetTransactionsWithInvalidPeriod_ShouldThrowArgumentOutOfRangeException()
		{
			var controller = new TransactionsController(_autoMapper.Create(_db), _service);
			controller.AddControllerContext(_user);

			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.GetList(null, new DateTime(2019, 6, 4), new DateTime(2019, 6, 1)));
		}

		[Test]
        public async Task AddTransaction_ShouldCallAddMethodWithRightArguments()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var transaction = new Api.Dto.Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                AccountId = account.Id,
                CategoryId = category.Id,
                BankCategory = "Bank category #1",
                Approved = false,
                Mcc = 111
            };

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);
            _transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(new Transaction());

            var controller = new TransactionsController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.Add(transaction);

            _transactions.Verify(x => x.Add(It.Is<Transaction>(t => 
                t.DateTime == transaction.DateTime &&
                t.Amount == transaction.Amount &&
                t.Description == transaction.Description &&
                t.Account.Id == transaction.AccountId &&
                t.Category.Id == transaction.CategoryId &&
                t.BankCategory == transaction.BankCategory &&
                t.Approved == transaction.Approved &&
                t.Mcc == transaction.Mcc
                )), Times.Once);
        }

        [Test]
        public async Task PatchTransactionCategory_ShouldSetCategory()
        {
            var bank = await _dal.Banks.Add(new Bank() {Title = "Bank #1", User = _user});
            var account = await _dal.Accounts.Add(new BankAccount() {Transactions = new List<Transaction>(), Bank = bank});
            var transaction = await _dal.Transactions.Add(new Transaction()
                {DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account});
            var category = await _dal.Categories.Add(new Category() {Title = "Category #1", User = _user});

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);
            _transactions.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(transaction);
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync(transaction);

            var controller = new TransactionsController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.Patch(transaction.Id,
                new JsonPatchDocument<Api.Dto.Transaction>(new List<Operation<Api.Dto.Transaction>>()
                    {
                        new Operation<Api.Dto.Transaction>("replace", "/categoryId", null, category.Id)
                    },
                    new CamelCasePropertyNamesContractResolver()));

            Assert.AreEqual(category.Id, result.Value.CategoryId);
        }

        [Test]
        public async Task PatchTransactionNullCategory_ShouldClearCategory()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var transaction = await _dal.Transactions.Add(new Transaction()
            { DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account, Category = category });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);
            _transactions.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(transaction);
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync(transaction);

            var controller = new TransactionsController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.Patch(transaction.Id,
                new JsonPatchDocument<Api.Dto.Transaction>(new List<Operation<Api.Dto.Transaction>>()
                    {
                        new Operation<Api.Dto.Transaction>("replace", "/categoryId", null)
                    }, 
                    new CamelCasePropertyNamesContractResolver()));

            Assert.IsNull(result.Value.CategoryId);
        }
    }
}