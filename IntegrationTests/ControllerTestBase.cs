using Microsoft.AspNetCore.TestHost;
using MyFinanceServer.Api;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	public class ControllerTestBase : TestFixtureBase
	{
		protected HttpClient Client => _server.CreateClient();
		protected string _accessToken;
		protected TestServer _server;

		[OneTimeSetUp]
		public async Task HttpClientSetup()
		{
			_server = WebApi.Instance;
			Assert.NotNull(_server);

			await Login();
		}

		protected virtual async Task Login()
		{
			var content = new StringContent(JsonConvert.SerializeObject(
				new RegisterBindingModel() { Email = "test1@email.com", Password = "Password #1", ConfirmPassword = "Password #2" }), Encoding.UTF8, "application/json");
			var response = await Client.PostAsync("/api/v1/users/login", content);

			var dict = await response.ToDictionary();

			_accessToken = (string)dict["access_token"];
			Assert.IsTrue(_accessToken.Length > 32);
		}

		protected virtual HttpRequestMessage CreateAuthJsonRequest(string method, string url)
		{
			var request = new HttpRequestMessage(new HttpMethod(method), url);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

			return request;
		}

		protected virtual HttpRequestMessage CreateJsonRequest(string method, string url)
		{
			var request = new HttpRequestMessage(new HttpMethod(method), url);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return request;
		}

		protected virtual Task<HttpResponseMessage> SendRequest(HttpRequestMessage request) => Client.SendAsync(request, CancellationToken.None);
	}
}
