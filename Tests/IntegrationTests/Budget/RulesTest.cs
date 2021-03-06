﻿using NUnit.Framework;
using SlidFinance.Domain;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class RulesTest : ControllerTestBase
	{
		[Test]
		public async Task GetGeneratedRules_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new UserCategory("Category #1", 0);
			await _dal.Categories.Add(category);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();

			for (int i = 0; i < 5; i++)
			await _dal.Transactions.Add(new Transaction() { Account = account, Category = category, Amount = 1, Approved = true,
				BankCategory = "Bank category #1", Description = "Description #1", MccId = 1000, DateTime = DateTime.Today });

			for (int i = 0; i < 5; i++)
				await _dal.Transactions.Add(new Transaction() { Account = account, Category = category, Amount = 1, Approved = true,
					BankCategory = "Bank category #2", Description = "Description #2", MccId = 2, DateTime = DateTime.Today });

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/rules/generated", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task GetRuleList_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			await _db.SaveChangesAsync();
			var category = new UserCategory("Category #1", 0);
			await _dal.Categories.Add(category);
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();
			var rule1 = new Rule(account, "Bank category #1", category, "Description #1", 5000, 0);
			await _dal.Rules.Add(rule1);
			var rule2 = new Rule(account, "Bank category #2", category, "Description #2", 5001, 1);
			await _dal.Rules.Add(rule2);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/rules/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddRule_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new UserCategory("Category #1", 0);
			await _dal.Categories.Add(category);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/rules/", _accessToken, new Dto.Rule() { AccountId = account.Id,
				BankCategory = "Bank category #1", CategoryId = category.Id, Description = "Description #1", MccId = 5000 });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateRule_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new UserCategory("Category #1", 0);
			await _dal.Categories.Add(category);
			var rule = new Rule(account, "Bank category #1", category, "Description #1", 1000, 0);
			await _dal.Rules.Add(rule);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/rules/" + rule.Id, _accessToken, new Dto.Rule
			{
				Id = rule.Id,
				AccountId = account.Id,
				BankCategory = "Bank category #2",
				CategoryId = category.Id,
				Description = "Description #2",
				MccId = 2000
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
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			var category = new UserCategory("Category #1", 0);
			await _dal.Categories.Add(category);
			var rule = new Rule(account, "Bank category #1", category, "Description #1", 5000, 0);
			await _dal.Rules.Add(rule);
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/rules/" + rule.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
