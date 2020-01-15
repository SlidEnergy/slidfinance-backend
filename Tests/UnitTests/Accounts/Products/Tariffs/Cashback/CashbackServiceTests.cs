using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;
using SlidFinance.App.Analysis;

namespace SlidFinance.WebApi.UnitTests
{
    public class CashbackServiceTests : TestsBase
    {
        private CashbackService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CashbackService(_db);
        }

        [Test]
        public async Task WhichCardToPay_ShouldReturnList()
        {
			var bank1 = await _db.CreateBank();
            var account1 = await _db.CreateAccount(_user);
            var product1 = await _db.CreateProduct(_user, bank1.Id);
            var tariff1 = await _db.CreateTariff(product1.Id);
            var category1 = await _db.CreateCashbackCategory(tariff1.Id);
            category1.Type = CashbackCategoryType.BaseCashback;
            var cashback1 = await _db.CreateCashback(category1.Id);
            cashback1.Percent = 0.01f;
            var category2 = await _db.CreateCashbackCategory(tariff1.Id);
            category2.Type = CashbackCategoryType.IncreasedCashback;
            var cashbackMcc2 = await _db.CreateCashbackCategoryMcc(category2.Id, 4812);
            var cashback2 = await _db.CreateCashback(category2.Id);
            cashback2.Percent = 0.03f;
            var category3 = await _db.CreateCashbackCategory(tariff1.Id);
            category3.Type = CashbackCategoryType.IncreasedCashback;
            var cashbackMcc3 = await _db.CreateCashbackCategoryMcc(category3.Id, 4814);
            var cashback3 = await _db.CreateCashback(category3.Id);
            cashback3.Percent = 0.05f;
            var category4 = await _db.CreateCashbackCategory(tariff1.Id);
            category4.Type = CashbackCategoryType.NoCashback;
            var cashbackMcc4 = await _db.CreateCashbackCategoryMcc(category4.Id, 4816);

            account1.BankId = bank1.Id;
            account1.ProductId = product1.Id;
            account1.SelectedTariffId = tariff1.Id;

            var bank2 = await _db.CreateBank();
            var account2 = await _db.CreateAccount(_user);
            var product2 = await _db.CreateProduct(_user, bank2.Id);
            var tariff2 = await _db.CreateTariff(product2.Id);
            var category21 = await _db.CreateCashbackCategory(tariff2.Id);
            category21.Type = CashbackCategoryType.BaseCashback;
            var cashback21 = await _db.CreateCashback(category21.Id);
            cashback21.Percent = 0.01f;
            var category22 = await _db.CreateCashbackCategory(tariff2.Id);
            category22.Type = CashbackCategoryType.IncreasedCashback;
            var cashbackMcc22 = await _db.CreateCashbackCategoryMcc(category22.Id, 4812);
            var cashback22 = await _db.CreateCashback(category22.Id);
            cashback22.Percent = 0.05f;
            var category23 = await _db.CreateCashbackCategory(tariff2.Id);
            category23.Type = CashbackCategoryType.IncreasedCashback;
            var cashbackMcc23 = await _db.CreateCashbackCategoryMcc(category23.Id, 4814);
            var cashback23 = await _db.CreateCashback(category23.Id);
            cashback23.Percent = 0.03f;
            account2.BankId = bank2.Id;
            account2.ProductId = product2.Id;
            account2.SelectedTariffId = tariff2.Id;

            await _db.SaveChangesAsync();

            var list = await _service.WhichCardToPay(_user.Id, "4812 4814 4816");

            Assert.AreEqual(3, list.Count);
            Assert.NotNull(list.Where(x => x.SearchPart == "4812" && x.BankTitle == bank2.Title && x.AccountTitle == account2.Title && 
                x.CategoryTitle == category22.Title && x.Percent == cashback22.Percent).FirstOrDefault());
            Assert.NotNull(list.Where(x => x.SearchPart == "4814" && x.BankTitle == bank1.Title && x.AccountTitle == account1.Title &&
                x.CategoryTitle == category3.Title && x.Percent == cashback3.Percent).FirstOrDefault());
            Assert.NotNull(list.Where(x => x.SearchPart == "4816" && x.BankTitle == bank2.Title && x.AccountTitle == account2.Title &&
                x.CategoryTitle == category21.Title && x.Percent == cashback21.Percent).FirstOrDefault());
        }
	}
}