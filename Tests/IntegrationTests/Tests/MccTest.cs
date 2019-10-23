using NUnit.Framework;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class MccTest : ControllerTestBase
	{
		[Test]
		public async Task GetMccList_ShouldReturnContent()
		{
			await _dal.Mcc.Add(new Mcc()
			{
				Code = "1111",
				Title = "Category #1",
				Description = "Description #1",
				Category = MccCategory.Airlines
			});
			await _dal.Mcc.Add(new Mcc()
			{
				Code = "2222",
				Title = "Category #2",
				Description = "Description #2",
				Category = MccCategory.Airlines
			});

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/mcc/");
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}
	}
}
