using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class RulesControllerTests : TestsBase
    {
        RulesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new RulesService(_mockedDal);
        }

        [Test]
        public async Task GetRules_ShouldReturnList()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            var category = await _dal.Categories.Add(new Category() {Title = "Category #1", User = _user});

            var rule1 = await _dal.Rules.Add(new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555, Order = 1
            });
            var rule2 = await _dal.Rules.Add(new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 3333,
                Order = 2
            });

            _rules.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Rule>() { rule1, rule2 });

            var controller = new RulesController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}