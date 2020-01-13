using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;

namespace SlidFinance.WebApi.UnitTests
{
    public class TariffsServiceTests : TestsBase
    {
        private ProductTariffsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new ProductTariffsService(_db);
        }

        [Test]
        public async Task GetList_ShouldReturnList()
        {
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);

			var tariff1 = await _db.CreateTariff(_user, product.Id);
			var tariff2 = await _db.CreateTariff(_user, product.Id);

			var list = await _service.GetListWithAccessCheckAsync(_user.Id, product.Id);

			Assert.AreEqual(2, list.Count);
        }

		[Test]
		public async Task Add_ShouldBeAdded()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);

			var tariff = new ProductTariff() { ProductId = product.Id, Title = "Tariff #1", Type = ProductType.Card };
			await _service.Add(_user.Id, tariff);


			var addedTariff = _db.Tariffs
				.Where(t => t.ProductId == product.Id)
				.FirstOrDefault();

			Assert.NotNull(addedTariff);
		}

		[Test]
		public async Task Update_ShouldBeUpdated()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);

			var model = new ProductTariff() { Id = tariff.Id, ProductId = product.Id, Title = "Tariff #1" };
			await _service.Edit(_user.Id, model);

			var updatedTariff = _db.Tariffs
				.Where(t => t.Id == tariff.Id)
				.FirstOrDefault();

			Assert.AreEqual(model.Title, updatedTariff.Title);
		}
	}
}