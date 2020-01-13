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
	public class ProductControllerTests : TestsBase
    {
		private ProductsController _controller;
		private Mock<IProductsService> _service;

		[SetUp]
        public void Setup()
        {
            _service = new Mock<IProductsService>();
			_controller = new ProductsController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetList_ShouldCallMethod()
        {
            var bank = new Bank() { Title = "Bank #1" };
            var product1 = new Product() { Title = "Product #1", Bank = bank };
            var product2 = new Product() { Title = "Product #2", Bank = bank };

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>())).ReturnsAsync(new List<Product>() { product1, product2 });

            var result = await _controller.GetList();

            Assert.AreEqual(2, result.Value.Count());

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id)));
		}

		[Test]
		public async Task AddProduct_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Dto.Product() { Title = "Product #1", BankId = bank.Id };

			_service.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Product>())).ReturnsAsync((string u, Product x) => x);

			var result = await _controller.Add(product);

			_service.Verify(x => x.Add(It.Is<string>(u => u == _user.Id), It.Is<Product>(p => p.Title == product.Title && product.BankId == bank.Id)));
		}

		[Test]
		public async Task UpdateProduct_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Dto.Product() { Id = 1, Title = "Product #1", BankId = bank.Id };

			_service.Setup(x => x.Edit(It.IsAny<string>(), It.IsAny<Product>())).ReturnsAsync((string u, Product x) => x);

			var result = await _controller.Update(product.Id, product);

			_service.Verify(x => x.Edit(It.Is<string>(u => u == _user.Id), It.Is<Product>(p => p.Id == product.Id && p.Title == product.Title && product.BankId == bank.Id)));
		}

		[Test]
		public async Task PatchProduct_ShouldCallMethod()
		{
			var bank = new Bank() { Id = 1, Title = "Bank #1" };
			var product = new Product() { Id = 1, Title = "Product #1", BankId = bank.Id };

			_service.Setup(x => x.GetByIdWithAccessCheck(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(product);
			_service.Setup(x => x.Edit(It.IsAny<string>(), It.IsAny<Product>())).ReturnsAsync((string u, Product x) => x);

			var result = await _controller.PatchProduct(product.Id, new JsonPatchDocument<Dto.Product>(
				new List<Operation<Dto.Product>>() 
					{ 
						new Operation<Dto.Product>() { op = "replace", path = "/title", value = "Product #2" } 
					}, 
					new CamelCasePropertyNamesContractResolver()));

			_service.Verify(x => x.Edit(It.Is<string>(u => u == _user.Id), It.Is<Product>(p => p.Id == product.Id && p.Title == "Product #2" && product.BankId == bank.Id)));
		}
	}
}