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