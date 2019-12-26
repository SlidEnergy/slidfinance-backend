using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class TransactionsTest : ControllerTestBase
	{
		[Test]
		public async Task GetTransactionsList_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);

			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			await _db.SaveChangesAsync();

			var tx1 = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx1);
			var tx2 = new Transaction() { Account = account, BankCategory = "Bank category #2", Description = "Description #2", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx2);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/transactions/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddTransaction_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			var mcc = new Mcc() { Code = "0100" };
			_db.Mcc.Add(mcc);
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/transactions/", _accessToken, new Dto.Transaction() { AccountId = account.Id,
				BankCategory = "Bank category #1", Description = "Description #1", Mcc = Convert.ToInt32(mcc.Code), DateTime = DateTime.Today });
			var response = await SendRequest(request);

			response.IsSuccess();
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task PatchAccount_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var tx = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("PATCH", "/api/v1/transactions/" + account.Id, _accessToken,
				new[] { new { op = "replace", path = "/description", value = "Description #2" } });

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteRule_ShouldNoContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var tx = new Transaction() { Account = account, BankCategory = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today };
			await _dal.Transactions.Add(tx);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/transactions/" + tx.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
