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
			var tariff = await _db.CreateTariff(product.Id);

			var category1 = await _db.CreateCashbackCategory(product.Id);
			var category2 = await _db.CreateCashbackCategory(product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/cashbackcategories?tariffId=" + tariff.Id, _accessToken);
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
			var tariff = await _db.CreateTariff(product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/cashbackcategories/", _accessToken, new CashbackCategory()
			{
				TariffId = tariff.Id,
				Title = "Tariff #1",
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task Update_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(product.Id);
			var category = await _db.CreateCashbackCategory(tariff.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/cashbackcategories/" + tariff.Id, _accessToken, new CashbackCategory
			{
				Id = category.Id,
				Title = "Tariff #2",
				TariffId = tariff.Id,
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}
	}
}
