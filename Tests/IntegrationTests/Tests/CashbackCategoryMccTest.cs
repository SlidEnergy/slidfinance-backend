using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class CashbackCategoryMccTest : ControllerTestBase
	{
		[Test]
		public async Task GetList_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);
			var mcc1 = await _db.CreateMcc("1111");
			var mcc2 = await _db.CreateMcc("2222");
			var category = await _db.CreateCashbackCategory(_user, tariff.Id);
			var cashbackMcc1 = await _db.CreateCashbackCategoryMcc(_user, category.Id, mcc1.Id);
			var cashbackMcc2 = await _db.CreateCashbackCategoryMcc(_user, category.Id, mcc2.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/cashback/categories/" + category.Id + "/mcc", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task Add_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);
			var mcc = await _db.CreateMcc("1111");
			var category = await _db.CreateCashbackCategory(_user, tariff.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/cashback/categories/" + category.Id + "/mcc", _accessToken, new Dto.CashbackCategoryMcc()
			{
				MccId = mcc.Id,
				CategoryId = category.Id
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

	}
}
