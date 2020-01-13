using Moq;
using SlidFinance.WebApi;
using SlidFinance.App;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Collections.Generic;

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
            var bank1 = await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
            });
			var bank2 = await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #2",
            });

            _service.Setup(x => x.GetLis()).ReturnsAsync(new List<Bank>() { bank1, bank2 });

            var result = await _controller.GetList();

			_service.Verify(x => x.GetLis());
		}
    }
}