using MyFinanceServer.Core;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	[TestFixture]
	public class AccountsTest : ControllerTestBase
	{
		[Test]
		public async Task GetAccountList_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account1 = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account1);
			var account2 = new BankAccount(bank, "Account #2", "Code #2", 200, 10);
			await _dal.Accounts.Add(account2);

			var request = CreateAuthJsonRequest("GET", "/api/v1/accounts/");
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddAccount_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);

			var request = CreateAuthJsonRequest("POST", "/api/v1/accounts/", new Api.Dto.BankAccount() { BankId = bank.Id, Balance = 100, Code = "Code #1",
				Title = "Account #1", CreditLimit = 50 });
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

			var request = CreateAuthJsonRequest("PATCH", "/api/v1/accounts/" + account.Id, new[] { new { op = "replace", path = "/title", value = "Account #2" } });

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateAccount_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);

			var request = CreateAuthJsonRequest("PUT", "/api/v1/accounts/" + account.Id, new Api.Dto.BankAccount
			{
				Id = account.Id,
				Balance = 200,
				CreditLimit = 100,
				BankId	 = bank.Id,
				Code = "Code #2",
				Title = "Account #2"
			});
			
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

			var request = CreateAuthJsonRequest("DELETE", "/api/v1/accounts/" + account.Id);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
