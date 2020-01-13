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
	public class TariffsControllerTests : TestsBase
    {
		private TariffsController _controller;
		private Mock<IProductTariffsService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<IProductTariffsService>();
			_controller = new TariffsController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetList_ShouldCallMethod()
        {
            var bank = new Bank() { Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff1 = new ProductTariff() { Title = "Tariff #1", ProductId = product.Id };
            var tariff2 = new ProductTariff() { Title = "Tariff #2", ProductId = product.Id };

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new List<ProductTariff>() { tariff1, tariff2});

            var result = await _controller.GetList(product.Id);

            Assert.AreEqual(2, result.Value.Count());

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id), It.Is<int>(p => p == product.Id)));
		}

		[Test]
		public async Task Add_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new Dto.ProductTariff() { Title = "Tariff #1", ProductId = product.Id };

			_service.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<ProductTariff>())).ReturnsAsync((string u, ProductTariff x) => x);

			var result = await _controller.Add(tariff);

			_service.Verify(x => x.Add(It.Is<string>(u => u == _user.Id), It.Is<ProductTariff>(p => p.Title == tariff.Title && tariff.ProductId == product.Id)));
		}

		[Test]
		public async Task Update_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new Dto.ProductTariff() { Title = "Tariff #1", ProductId = product.Id };

			_service.Setup(x => x.Edit(It.IsAny<string>(), It.IsAny<ProductTariff>())).ReturnsAsync((string u, ProductTariff x) => x);

			var result = await _controller.Update(product.Id, tariff);

			_service.Verify(x => x.Edit(It.Is<string>(u => u == _user.Id), It.Is<ProductTariff>(p => p.Id == tariff.Id && p.Title == tariff.Title && tariff.ProductId == product.Id)));
		}

		[Test]
		public async Task PatchProduct_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", Bank = bank };
			var tariff = new ProductTariff() { Title = "Tariff #1", ProductId = product.Id };

			_service.Setup(x => x.GetByIdWithAccessCheck(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(tariff);
			_service.Setup(x => x.Edit(It.IsAny<string>(), It.IsAny<ProductTariff>())).ReturnsAsync((string u, ProductTariff x) => x);

			var result = await _controller.Patch(tariff.Id, new JsonPatchDocument<Dto.ProductTariff>(
				new List<Operation<Dto.ProductTariff>>() 
					{ 
						new Operation<Dto.ProductTariff>() { op = "replace", path = "/title", value = "Product #2" } 
					}, 
					new CamelCasePropertyNamesContractResolver()));

			_service.Verify(x => x.Edit(It.Is<string>(u => u == _user.Id), It.Is<ProductTariff>(p => p.Id == tariff.Id && p.Title == "Product #2" && tariff.ProductId == product.Id)));
		}
	}
}