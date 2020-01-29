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

		[Test]
		public async Task Add_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Title = "Tariff #1", ProductId = product.Id };
			var category = new CashbackCategory() { Title = "Category #1", TariffId = tariff.Id };

			_service.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<CashbackCategory>())).ReturnsAsync((string u, CashbackCategory x) => x);

			var result = await _controller.Add(category);

			_service.Verify(x => x.Add(It.Is<string>(u => u == _user.Id), It.Is<CashbackCategory>(p => p.Title == category.Title && p.TariffId == tariff.Id)));
		}

		[Test]
		public async Task Update_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Id = 1, Title = "Tariff #1", ProductId = product.Id };
			var category = new CashbackCategory() { Id = 1, Title = "Category #1", TariffId = tariff.Id };

			_service.Setup(x => x.Edit(It.IsAny<string>(), It.IsAny<CashbackCategory>())).ReturnsAsync((string u, CashbackCategory x) => x);

			var result = await _controller.Update(category.Id, category);

			_service.Verify(x => x.Edit(It.Is<string>(u => u == _user.Id), It.Is<CashbackCategory>(p => p.Id == category.Id && p.Title == category.Title && p.TariffId == tariff.Id)));
		}
	}
}