using MyFinanceServer.Core;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	[TestFixture]
	public class RulesTest : ControllerTestBase
	{
		[Test]
		public async Task GetRuleList_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new Category("Category #1", 0, _user);
			await _dal.Categories.Add(category);
			var rule1 = new Rule(account, "Bank category #1", category, "Description #1", 5000, 0);
			await _dal.Rules.Add(rule1);
			var rule2 = new Rule(account, "Bank category #2", category, "Description #2", 5001, 1);
			await _dal.Rules.Add(rule2);

			var request = CreateAuthJsonRequest("GET", "/api/v1/rules/");
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddRule_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new Category("Category #1", 0, _user);
			await _dal.Categories.Add(category);

			var request = CreateAuthJsonRequest("POST", "/api/v1/rules/", new Api.Dto.Rule() { AccountId = account.Id,
				BankCategory = "Bank category #1", CategoryId = category.Id, Description = "Description #1", Mcc = 5000 });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}
	}
}
