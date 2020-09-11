using NUnit.Framework;
using SlidFinance.Domain;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class BanksTest : ControllerTestBase
	{
		[Test]
		public async Task GetBanksList_ShouldReturnContent()
		{
			var bank1 = new Bank("Bank #1");
			await _dal.Banks.Add(bank1);
			var bank2 = new Bank("Bank #2");
			await _dal.Banks.Add(bank2);
			var account1 = new BankAccount(bank1, "Account #1", "Code #1", 0, 0);
			await _dal.Accounts.Add(account1);
			var account2 = new BankAccount(bank2, "Account #2", "Code #2", 0, 0);
			await _dal.Accounts.Add(account2);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account1));
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account2));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/banks/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddBank_ShouldReturnContent()
		{
			var userName = Guid.NewGuid() + "@mail.com";
			var password = Guid.NewGuid().ToString().ToUpper() + Guid.NewGuid().ToString().ToLower();
			var user = await CreateUser(userName, password);
			await _manager.AddToRoleAsync(user, Role.Admin);

			await Login(userName, password);

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/banks/", _accessToken, new Bank () {  Title = "Bank #1" });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateBank_ShouldReturnContent()
		{
			var userName = Guid.NewGuid() + "@mail.com";
			var password = Guid.NewGuid().ToString().ToUpper() + Guid.NewGuid().ToString().ToLower();
			var user = await CreateUser(userName, password);
			await _manager.AddToRoleAsync(user, Role.Admin);

			await Login(userName, password);

			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/banks/" + bank.Id, _accessToken, new Bank
			{
				Id = bank.Id,
				Title = "Bank #2"
			});
			
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteBank_ShouldNoContent()
		{
			var userName = Guid.NewGuid() + "@mail.com";
			var password = Guid.NewGuid().ToString().ToUpper() + Guid.NewGuid().ToString().ToLower();
			var user = await CreateUser(userName, password);
			await _manager.AddToRoleAsync(user, Role.Admin);

			await Login(userName, password);

			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/banks/" + bank.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
