using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class TokenTest : ControllerTestBase
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public async Task RefreshToken_ShouldReturnTokens()
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/token/refresh", _accessToken, 
				new TokensCortage { Token = _accessToken, RefreshToken = _refreshToken });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("token"));
			Assert.IsTrue(dict.ContainsKey("refreshToken"));
		}
	}
}
