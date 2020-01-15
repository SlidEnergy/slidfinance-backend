using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;

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
	}
}