using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Serialization;

namespace SlidFinance.WebApi.UnitTests
{
	public class CashbackCategoriesControllerTests : TestsBase
    {
		private CashbackCategoriesController _controller;
		private Mock<ICashbackCategoriesService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<ICashbackCategoriesService>();
			_controller = new CashbackCategoriesController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetList_ShouldCallMethod()
        {
            var bank = new Bank() { Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Title = "Tariff #1", ProductId = product.Id };

            var category1 = new CashbackCategory() { Title = "Category #2", TariffId = tariff.Id };
			var category2 = new CashbackCategory() { Title = "Category #2", TariffId = tariff.Id };

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new List<CashbackCategory>() { category1, category2});

            var result = await _controller.GetList(tariff.Id);

            Assert.AreEqual(2, result.Value.Count());

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id), It.Is<int>(p => p == tariff.Id)));
		}
	}
}