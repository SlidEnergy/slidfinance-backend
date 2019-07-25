using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class RulesServiceTests : TestsBase
    {
        RulesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new RulesService(_mockedDal);
        }

        [Test]
        public async Task AddFirstRule_ShouldBeReturnRuleWithRightProperties()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });

            var rule = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555
            };

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _rules.Setup(x => x.Add(It.IsAny<Rule>())).ReturnsAsync((Rule x) => x);
            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(await _dal.Rules.GetListWithAccessCheck(_user.Id));

            var newRule = await _service.AddRule(_user.Id, account.Id, rule.BankCategory, rule.Category.Id, rule.Description, rule.Mcc);

            Assert.AreEqual(rule.Account.Id, newRule.Account.Id);
            Assert.AreEqual(rule.Category.Id, newRule.Category.Id);
            Assert.AreEqual(rule.BankCategory, newRule.BankCategory);
            Assert.AreEqual(rule.Description, newRule.Description);
            Assert.AreEqual(rule.Mcc, newRule.Mcc);
            Assert.AreEqual(0, newRule.Order);
        }

        [Test]
        public async Task AddSecondRule_ShouldBeReturnRuleWithRightProperties()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            var category = await _dal.Categories.Add(new Category() { Title = "Category #1", User = _user });
            await _dal.Rules.Add(new Rule() { Account = account });
            var rule = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555
            };

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _accounts.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(account);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _rules.Setup(x => x.Add(It.IsAny<Rule>())).ReturnsAsync((Rule x) => x);
            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(await _dal.Rules.GetListWithAccessCheck(_user.Id));

            var newRule = await _service.AddRule(_user.Id, account.Id, rule.BankCategory, rule.Category.Id, rule.Description, rule.Mcc);

            Assert.AreEqual(rule.Account.Id, newRule.Account.Id);
            Assert.AreEqual(rule.Category.Id, newRule.Category.Id);
            Assert.AreEqual(rule.BankCategory, newRule.BankCategory);
            Assert.AreEqual(rule.Description, newRule.Description);
            Assert.AreEqual(rule.Mcc, newRule.Mcc);
            Assert.AreEqual(1, newRule.Order);
        }
    }
}