using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.UnitTests
{
    public class BanksControllerTests : TestsBase
    {
		private BanksController _controller;

		[SetUp]
        public void Setup()
        {
            var service = new BanksService(_mockedDal);
			_controller = new BanksController(_autoMapper.Create(_db), service);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetBanks_ShouldReturnList()
        {
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = _user
            });
            await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = _user
            });

            _banks.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task GetEmptyBanksList_ShouldBeEmptyListReturned()
        {
            _banks.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _controller.GetList();

            Assert.AreEqual(0, result.Value.Count());
        }
    }
}