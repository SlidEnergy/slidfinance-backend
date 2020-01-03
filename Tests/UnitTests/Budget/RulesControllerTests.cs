using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class RulesControllerTests : TestsBase
    {
		private RulesController _controller;
		private Mock<IRulesService> _service;

		[SetUp]
        public void Setup()
        {
			_service = new Mock<IRulesService>();
			_controller = new RulesController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetRules_ShouldReturnList()
        {
            var bank = new Bank() { Title = "Bank #1" };
            var account = new BankAccount() { Title = "Account #1", Bank = bank };
            var category = new UserCategory() {Title = "Category #1"};

            var rule1 = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                MccId = 5555, Order = 1
            };
            var rule2 = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                MccId = 3333,
                Order = 2
            };

            _service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>())).ReturnsAsync(new List<Rule>() { rule1, rule2 });

            var result = await _controller.GetList();

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id)));

		}
    }
}