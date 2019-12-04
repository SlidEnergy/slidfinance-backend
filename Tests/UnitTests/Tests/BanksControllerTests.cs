using Moq;
using SlidFinance.WebApi;
using SlidFinance.App;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class BanksControllerTests : TestsBase
    {
		private BanksController _controller;
		private Mock<IBanksService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<IBanksService>();
			_controller = new BanksController(_autoMapper.Create(_db), _service.Object);
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

            _service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>())).ReturnsAsync(_user.Banks.ToList());

            var result = await _controller.GetList();

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id)));
		}
    }
}