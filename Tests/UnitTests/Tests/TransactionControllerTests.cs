using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using SlidFinance.WebApi;
using SlidFinance.App;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
	public class TransactionControllerTests : TestsBase
    {
		private TransactionsController _controller;
		private Mock<ITransactionsService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<ITransactionsService>();
			_controller = new TransactionsController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

		[Test]
		public async Task GetTransactionsForCategoryAndPeriod_ShouldReturnList()
		{
			var bank = new Bank() { Title = "Bank #1", User = _user };
			var account = new BankAccount() { Transactions = new List<Transaction>(), Bank = bank };
			var category = new Category() { Title = "Category #1", User = _user };
			var t1 = new Transaction()
			{
				DateTime = new DateTime(2019, 6, 1),
				Amount = 10,
				Description = "Description #1",
				Account = account,
				Category = category
			};
			var t2 = await _dal.Transactions.Add(new Transaction()
			{
				DateTime = new DateTime(2019, 6, 2),
				Amount = 5,
				Description = "Description #2",
				Account = account,
				Category = category
			});

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
				.ReturnsAsync(new List<Transaction>() { t1, t2 });

			var result = await _controller.GetList(null, category.Id, new DateTime(2019, 6, 2), new DateTime(2019, 6, 4));

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id), It.Is<int?>(c => c == null), 
				It.Is<int?>(c => c == category.Id), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()));
		}

		[Test]
        public async Task AddTransaction_ShouldCallAddMethodWithRightArguments()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var transaction = new Dto.Transaction()
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

			_service.Setup(x => x.AddTransaction(It.IsAny<string>(), It.IsAny<Transaction>())).ReturnsAsync(new Transaction());

            var result = await _controller.Add(transaction);

			_service.Verify(x => x.AddTransaction(It.Is<string>(u => u == _user.Id),
				It.Is<Transaction>(t => 
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
			var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
			var transaction = await _dal.Transactions.Add(new Transaction()
                {DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account});

			_service.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(transaction);
			_service.Setup(x => x.PatchTransaction(It.IsAny<string>(), It.IsAny<Transaction>())).ReturnsAsync(transaction);

            var result = await _controller.Patch(transaction.Id,
                new JsonPatchDocument<Dto.Transaction>(new List<Operation<Dto.Transaction>>()
                    {
                        new Operation<Dto.Transaction>("replace", "/categoryId", null, category.Id)
                    },
                    new CamelCasePropertyNamesContractResolver()));

			_service.Verify(x => x.GetById(It.Is<string>(u => u == _user.Id), It.Is<int>(t => t == transaction.Id)));
			_service.Verify(x => x.PatchTransaction(It.Is<string>(u => u == _user.Id), It.Is<Transaction>(t => t.CategoryId == category.Id)));
		}

        [Test]
        public async Task PatchTransactionNullCategory_ShouldClearCategory()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Transactions = new List<Transaction>(), Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            var transaction = await _dal.Transactions.Add(new Transaction()
            { DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account, Category = category });

			_service.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(transaction);
			_service.Setup(x => x.PatchTransaction(It.IsAny<string>(), It.IsAny<Transaction>())).ReturnsAsync(transaction);

			var result = await _controller.Patch(transaction.Id,
				new JsonPatchDocument<Dto.Transaction>(new List<Operation<Dto.Transaction>>()
					{
						new Operation<Dto.Transaction>("replace", "/categoryId", null)
					},
					new CamelCasePropertyNamesContractResolver()));

			_service.Verify(x => x.GetById(It.Is<string>(u => u == _user.Id), It.Is<int>(t => t == transaction.Id)));
			_service.Verify(x => x.PatchTransaction(It.Is<string>(u => u == _user.Id), It.Is<Transaction>(t => t.CategoryId == null)));
        }
    }
}