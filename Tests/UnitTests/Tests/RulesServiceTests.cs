using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class RulesServiceTests : TestsBase
    {
        RulesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new RulesService(_mockedDal, _db);
        }

        [Test]
        public async Task AddFirstRule_ShouldBeReturnRuleWithRightProperties()
        {
            var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
            var account = new BankAccount() { Title = "Account #1", Bank = bank };
			_db.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
            var category = new Category() { Title = "Category #1" };
			_db.Categories.Add(category);
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			await _db.SaveChangesAsync();

            var rule = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555
            };

            _rules.Setup(x => x.Add(It.IsAny<Rule>())).ReturnsAsync((Rule x) => x);

            var newRule = await _service.AddRule(_user.Id, account.Id, rule.BankCategory, rule.Category.Id, rule.Description, rule.Mcc);

			_rules.Verify(x => x.Add(It.Is<Rule>(r => r.Account.Id == rule.Account.Id &&
				r.Category.Id == rule.Category.Id &&
				r.BankCategory == rule.BankCategory && 
				r.Description == rule.Description &&
				r.Mcc == rule.Mcc &&
				r.Order == 0)));
        }

        [Test]
        public async Task AddSecondRule_ShouldBeReturnRuleWithRightProperties()
        {
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = new BankAccount() { Title = "Account #1", Bank = bank };
			_db.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			var category = new Category() { Title = "Category #1" };
			_db.Categories.Add(category);
			_db.TrusteeCategories.Add(new TrusteeCategory(_user, category));
			var rule = new Rule() { Account = account, Category = category };
			_db.Rules.Add(rule);
			await _db.SaveChangesAsync();
			var newRule = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555
            };

            _rules.Setup(x => x.Add(It.IsAny<Rule>())).ReturnsAsync((Rule x) => x);

            var addedRule = await _service.AddRule(_user.Id, account.Id, newRule.BankCategory, newRule.Category.Id, newRule.Description, newRule.Mcc);

			_rules.Verify(x => x.Add(It.Is<Rule>(r => r.Account.Id == newRule.Account.Id &&
				  r.Category.Id == newRule.Category.Id &&
				  r.BankCategory == newRule.BankCategory &&
				  r.Description == newRule.Description &&
				  r.Mcc == newRule.Mcc &&
				  r.Order == 1)));
		}
    }
}