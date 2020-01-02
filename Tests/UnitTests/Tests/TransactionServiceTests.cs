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
		private ITransactionsService _service;
		private Mock<IAccountsService> _accountsService;

		[SetUp]
        public void Setup()
        {
			_accountsService = new Mock<IAccountsService>();
			_service = new TransactionsService(_mockedDal, _db);
		}

		[Test]
		public async Task GetTransactions_ShouldReturnList()
		{
			var bank = new Bank("Bank #1");
			_db.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			_db.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account ));
			var t1 = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(t1);
			var t2 = new Transaction() { Account = account, BankCategory = "Bank category #2", Description = "Description #2", DateTime = DateTime.Today };
			await _dal.Transactions.Add(t2);

			await _db.SaveChangesAsync();

			var list = await _service.GetListWithAccessCheckAsync(_user.Id);

			Assert.AreEqual(2, list.Count);
		}

		[Test]
		public async Task AddTransaction_ShouldCallAddMethodWithRightArguments()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = new BankAccount() { Code = "Code #1", Bank = bank };
			_db.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			var transaction = new Transaction()
				{ DateTime = DateTime.Now, Amount = 10, Description = "Description #1", UserDescription = "User description #1", Account = account };
			_db.Transactions.Add(transaction);
			await _db.SaveChangesAsync();

			_transactions.Setup(x => x.Add(It.IsAny<Transaction>())).ReturnsAsync(transaction);

			await _service.AddTransaction(_user.Id, transaction);

			_transactions.Verify(x => x.Add(It.Is<Transaction>(t => t.UserDescription == transaction.UserDescription)));
		}

		[Test]
		public async Task PatchTransaction_ShouldSetCategory()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = new BankAccount() { Code = "Code #1", Bank = bank };
			_db.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			var transaction = new Transaction()
				{ DateTime = DateTime.Now, Amount = 10, Description = "Description #1", UserDescription = "User description #1", Account = account};
			_db.Transactions.Add(transaction);
			await _db.SaveChangesAsync();

			_transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync(transaction);

			await _service.PatchTransaction(_user.Id, transaction);

			_transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.UserDescription == transaction.UserDescription)));
		}

		[Test]
		public void GetTransactionsWithInvalidArguments_ShouldThrowArgumentOutOfRangeException()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetListWithAccessCheckAsync(_user.Id, -1));
		}

		[Test]
		public void GetTransactionsWithInvalidPeriod_ShouldThrowArgumentOutOfRangeException()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.GetListWithAccessCheckAsync(_user.Id, null, null, new DateTime(2019, 6, 4), new DateTime(2019, 6, 1)));
		}
	}
}