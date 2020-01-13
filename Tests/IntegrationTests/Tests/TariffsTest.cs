using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class TariffsTest : ControllerTestBase
	{
		[Test]
		public async Task GetList_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff1 = await _db.CreateTariff(_user, product.Id);
			var tariff2 = await _db.CreateTariff(_user, product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/products/" + product.Id + "/tariffs", _accessToken);
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

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/products/" + product.Id + "/tariffs", _accessToken, new Dto.ProductTariff() { 
				ProductId = product.Id, 
				Title = "Tariff #1",
				Type = ProductType.Card
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task Patch_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("PATCH", "/api/v1/products/" + product.Id + "/tariffs/" + tariff.Id, _accessToken, new[] { new { op = "replace", path = "/title", value = "Tariff #2" } });

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
			var tariff = await _db.CreateTariff(_user, product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/products/" + product.Id + "/tariffs/" + tariff.Id, _accessToken, new Dto.ProductTariff
			{
				Id = product.Id,
				Title = "Tariff #2",
				ProductId = product.Id,
				Type = ProductType.Card
			});
			
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task Delete_ShouldNoContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);
			var tariff = await _db.CreateTariff(_user, product.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/products/" + product.Id + "/tariffs/" + tariff.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
