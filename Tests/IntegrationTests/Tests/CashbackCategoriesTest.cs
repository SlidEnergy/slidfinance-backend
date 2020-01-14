using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class CashbackCategoriesTest : ControllerTestBase
	{
		[Test]
		public async Task GetList_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);

			var category1 = await _db.CreateCashbackCategory(_user, product.Id);
			var category2 = await _db.CreateCashbackCategory(_user, product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/tariffs/" + tariff.Id + "/cashback/categories", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}
	}
}
