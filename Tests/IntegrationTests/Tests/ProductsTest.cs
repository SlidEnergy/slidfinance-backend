using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class ProductsTest : ControllerTestBase
	{
		[Test]
		public async Task GetProductList_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product1 = await _db.CreateProduct(_user, bank.Id);
			var product2 = await _db.CreateProduct(_user, bank.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/products/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddProduct_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/products/", _accessToken, new Dto.Product() { 
				BankId = bank.Id, 
				Title = "Product #1",
				Image = "Image #2",
				IsPublic = true,
				Approved = true,
				Type = ProductType.Card
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task PatchProduct_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("PATCH", "/api/v1/products/" + product.Id, _accessToken, new[] { new { op = "replace", path = "/title", value = "Product #2" } });

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateProduct_ShouldReturnContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/products/" + product.Id, _accessToken, new Dto.Product
			{
				Id = product.Id,
				Title = "Product #2",
				BankId = bank.Id,
				Image = "Image #2",
				IsPublic = true,
				Approved = true,
				Type = ProductType.Card
			});
			
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteProduct_ShouldNoContent()
		{
			var bank = await _db.CreateBank();
			var product = await _db.CreateProduct(_user, bank.Id);

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/products/" + product.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
