using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;

namespace SlidFinance.WebApi.UnitTests
{
    public class ProductServiceTests : TestsBase
    {
        private ProductsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new ProductsService(_db);
        }

        [Test]
        public async Task GetProduct_ShouldReturnList()
        {
			var bank = await _db.CreateBank();
			var product1 = await _db.CreateProduct(_user, bank.Id);
			var product2 = await _db.CreateProduct(_user, bank.Id);

			var list = await _service.GetListWithAccessCheckAsync(_user.Id);

			Assert.AreEqual(2, list.Count);
        }

		[Test]
		public async Task AddProduct_ShouldBeAdded()
		{
			var bank = await _db.CreateBank();

			var product = new Product() { BankId = bank.Id, Title = "Product #1" };
			await _service.AddProduct(_user.Id, product);

			var addedProduct = _db.TrusteeProducts
				.Where(t => t.TrusteeId == _user.TrusteeId)
				.Join(_db.Products, t => t.ProductId, p => p.Id, (t, p) => p)
				.FirstOrDefault();

			Assert.NotNull(addedProduct);
		}

		[Test]
		public async Task UpdateProduct_ShouldBeUpdated()
		{
			var bank = await _db.CreateBank();

			var product = await _db.CreateProduct(_user, bank.Id);
   
			var model = new Product() { Id = product.Id, BankId = bank.Id, Title = "Product #1" };
			await _service.EditProduct(_user.Id, model);

			var updatedProduct = _db.TrusteeProducts
				.Where(t => t.TrusteeId == _user.TrusteeId)
				.Join(_db.Products, t => t.ProductId, p => p.Id, (t, p) => p)
				.FirstOrDefault();

			Assert.AreEqual(model.Title, updatedProduct.Title);
		}
	}
}