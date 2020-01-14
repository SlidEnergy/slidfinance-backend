using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;

namespace SlidFinance.WebApi.UnitTests
{
    public class CashbackCategoryMccServiceTests : TestsBase
    {
        private CashbackCategoryMccService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CashbackCategoryMccService(_db);
        }

        [Test]
        public async Task GetList_ShouldReturnList()
        {
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
            var tariff = await _db.CreateTariff(_user, product.Id);
            var category = await _db.CreateCashbackCategory(_user, tariff.Id);

            await _db.CreateCashbackCategoryMcc(_user, category.Id, 4812);
			await _db.CreateCashbackCategoryMcc(_user, category.Id, 4812);

			var list = await _service.GetListWithAccessCheckAsync(_user.Id, category.Id);

			Assert.AreEqual(2, list.Count);
        }

        [Test]
        public async Task Add_ShouldBeAdded()
        {
            var bank = await _db.CreateBank();
            var product = await _db.CreateProduct(_user, bank.Id);
            var tariff = await _db.CreateTariff(_user, product.Id);
            var category = await _db.CreateCashbackCategory(_user, tariff.Id);

            var categoryMcc = new CashbackCategoryMcc() { CategoryId = category.Id, MccCode = 6812};
            await _service.Add(_user.Id, categoryMcc);


            var addedTariff = _db.CashbackCategoryMcc
                .Where(t => t.CategoryId == product.Id)
                .FirstOrDefault();

            Assert.NotNull(addedTariff);
        }
    }
}