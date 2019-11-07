using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class TransactionServiceTests: TestsBase
    {
		ITransactionsService _service;

		[SetUp]
        public void Setup()
        {
			_service = new TransactionsService(_mockedDal);
		}

		[Test]
		public async Task GetTransactions_ShouldReturnList()
		{
			var transaction = new Transaction()
				{ DateTime = DateTime.Now, Amount = 10, Description = "Description #1", UserDescription = "User description #1" };

			_users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
			_transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Transaction> { transaction });

			await _service.GetList(_user.Id);

			_transactions.Verify(x => x.GetListWithAccessCheck(It.Is<string>(u => u == _user.Id)));
		}

		[Test]
		public async Task AddTransaction_ShouldCallAddMethodWithRightArguments()
		{
			var bank = new Bank() { Title = "Bank #1", User = _user };
			var account = new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank };
			var transaction = new Transaction()
				{ DateTime = DateTime.Now, Amount = 10, Description = "Description #1", UserDescription = "User description #1", Account = account };

			_users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
			_transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(transaction);
			_accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);

			await _service.AddTransaction(_user.Id, transaction);

			_transactions.Verify(x => x.Add(It.Is<Transaction>(t => t.UserDescription == transaction.UserDescription)));
		}

		[Test]
		public async Task PatchTransaction_ShouldSetCategory()
		{
			var bank = new Bank() { Title = "Bank #1", User = _user };
			var account = new BankAccount() { Code = "Code #1", Transactions = new List<Transaction>(), Bank = bank };
			var transaction = new Transaction()
				{ DateTime = DateTime.Now, Amount = 10, Description = "Description #1", UserDescription = "User description #1", Account = account};

			_users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
			_transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync(transaction);
			_accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);

			await _service.PatchTransaction(_user.Id, transaction);

			_transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.UserDescription == transaction.UserDescription)));
		}
	}
}