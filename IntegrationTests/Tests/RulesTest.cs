using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	[TestFixture]
	public class RulesTest : ControllerTestBase
	{
		[Test]
		public async Task GetRoom_ShouldReturnContent()
		{
			var request = CreateAuthJsonRequest("GET", "/api/v1/rules/" + 1);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.NotNull(dict["id"]);
		}
	}
}
