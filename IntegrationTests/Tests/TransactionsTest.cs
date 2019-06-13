using MyFinanceServer.Core;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	[TestFixture]
	public class TransactionsTest : ControllerTestBase
	{
		[Test]
		public async Task GetTransactionsList_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);

			var tx1 = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx1);
			var tx2 = new Transaction() { Account = account, BankCategory = "Bank category #2", Description = "Description #2", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx2);

			var request = CreateAuthJsonRequest("GET", "/api/v1/transactions/");
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddTransaction_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);

			var request = CreateAuthJsonRequest("POST", "/api/v1/transactions/", new Api.Dto.Transaction() { AccountId = account.Id,
				BankCategory = "Bank category #1", Description = "Description #1", Mcc = 5000, DateTime = DateTime.Today });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task PatchAccount_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var tx = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx);

			var request = CreateAuthJsonRequest("PATCH", "/api/v1/transactions/" + account.Id, new[] { new { op = "replace", path = "/description", value = "Description #2" } });

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteRule_ShouldNoContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var tx = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx);

			var request = CreateAuthJsonRequest("DELETE", "/api/v1/transactions/" + tx.Id);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
