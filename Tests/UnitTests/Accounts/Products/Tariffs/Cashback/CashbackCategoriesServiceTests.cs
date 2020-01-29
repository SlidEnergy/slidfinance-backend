using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;
using System;

namespace SlidFinance.WebApi.UnitTests
{
    public class CashbackCategoriesServiceTests : TestsBase
    {
        private CashbackCategoriesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CashbackCategoriesService(_db);
        }

        [Test]
        public async Task GetList_ShouldReturnList()
        {
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
            var tariff = await _db.CreateTariff(product.Id);
            await _db.CreateCashbackCategory(tariff.Id);
            await _db.CreateCashbackCategory(tariff.Id);

			var list = await _service.GetListWithAccessCheckAsync(_user.Id, product.Id);

			Assert.AreEqual(2, list.Count);
        }

		[Test]
		public async Task Add_ShouldBeAdded()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(product.Id);

			var model = new CashbackCategory() { TariffId = tariff.Id, Title = Guid.NewGuid().ToString()};
			await _service.Add(_user.Id, model);


			var addedModel = _db.CashbackCategories
				.Where(t => t.TariffId == tariff.Id)
				.FirstOrDefault();

			Assert.NotNull(addedModel);
		}

		[Test]
		public async Task Update_ShouldBeUpdated()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(product.Id);
			var category = await _db.CreateCashbackCategory(tariff.Id);

			var model = new CashbackCategory() { Id = tariff.Id, TariffId = tariff.Id, Title = Guid.NewGuid().ToString() };
			await _service.Edit(_user.Id, model);

			var updatedModel = _db.CashbackCategories
				.Where(t => t.Id == category.Id)
				.FirstOrDefault();

			Assert.AreEqual(model.Title, updatedModel.Title);
		}
	}
}