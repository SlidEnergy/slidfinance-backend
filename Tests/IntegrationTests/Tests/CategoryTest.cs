using NUnit.Framework;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class CategoryTest : ControllerTestBase
	{
		[Test]
		public async Task GetCategoryList_ShouldReturnContent()
		{
			var category1 = new Category("Category #1", 0, _user);
			await _dal.Categories.Add(category1);
			var category2 = new Category("Category #2", 0, _user);
			await _dal.Categories.Add(category2);
			_db.TrusteeCategories.Add(new TrusteeCategory() { TrusteeId = _user.TrusteeId, CategoryId = category1.Id });
			_db.TrusteeCategories.Add(new TrusteeCategory() { TrusteeId = _user.TrusteeId, CategoryId = category2.Id });
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/categories/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddCategory_ShouldReturnContent()
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/categories/", _accessToken, new Dto.Category() { Title = "Category #1" });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateRule_ShouldReturnContent()
		{
			var category = new Category("Category #1", 0, _user);
			await _dal.Categories.Add(category);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/categories/" + category.Id, _accessToken, new Dto.Category
			{
				Id = category.Id,
				Title = "Category #2"
			});

			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteRule_ShouldNoContent()
		{
			var category = new Category("Category #1", 0, _user);
			await _dal.Categories.Add(category);

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/categories/" + category.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
