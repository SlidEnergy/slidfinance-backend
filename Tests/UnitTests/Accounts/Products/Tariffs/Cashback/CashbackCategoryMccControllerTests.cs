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
	public class CashbackCategoryMccControllerTests : TestsBase
    {
		private CashbackCategoryMccController _controller;
		private Mock<ICashbackCategoryMccService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<ICashbackCategoryMccService>();
			_controller = new CashbackCategoryMccController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetList_ShouldCallMethod()
        {
            var bank = new Bank() { Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Id = 1, Title = "Tariff #1", ProductId = product.Id };
			var mcc1 = new Mcc() { Id = 1, Title = "Tariff #1", Code = "1111"};
			var mcc2 = new Mcc() { Id = 2, Title = "Tariff #1", Code = "2222" };
			var category = new CashbackCategory() { Id = 1, Title = "Category #2", TariffId = tariff.Id };

			var cashbackMcc1 = new CashbackCategoryMcc() { CategoryId = category.Id, MccId = mcc1.Id };
			var cashbackMcc2 = new CashbackCategoryMcc() { CategoryId = category.Id, MccId = mcc2.Id };

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new List<CashbackCategoryMcc>() { cashbackMcc1, cashbackMcc2});

            var result = await _controller.GetList(category.Id);

            Assert.AreEqual(2, result.Value.Count());

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id), It.Is<int>(p => p == category.Id)));
		}

		[Test]
		public async Task Add_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Id = 1, Title = "Tariff #1", ProductId = product.Id };
			var mcc = new Mcc() { Id = 1, Title = "Mcc #1", Code = "1111"};
			var category = new CashbackCategory() { Id = 1, Title = "Category #1", TariffId = tariff.Id };

			var categoryMcc = new Dto.CashbackCategoryMcc() { CategoryId = category.Id, MccId = mcc.Id };

			_service.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<CashbackCategoryMcc>())).ReturnsAsync((string u, CashbackCategoryMcc x) => x);

			var result = await _controller.Add(categoryMcc);

			_service.Verify(x => x.Add(It.Is<string>(u => u == _user.Id), It.Is<CashbackCategoryMcc>(p => p.CategoryId == category.Id && p.MccId == mcc.Id)));
		}
	}
}