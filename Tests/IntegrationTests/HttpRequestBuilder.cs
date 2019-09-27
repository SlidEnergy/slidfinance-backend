using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SlidFinance.WebApi.IntegrationTests
{
	public static class HttpRequestBuilder
	{
		public static HttpRequestMessage CreateGetJsonRequest(string url, string accessToken = null, object queryParams = null)
		{
			if (queryParams != null)
				url = $"{url}?{QueryBuilder.FromObject(queryParams)}";

			return CreateJsonRequest("GET", url, accessToken);
		}

		public static HttpRequestMessage CreateJsonRequest(string method, string url, string accessToken = null, object contentBody = null)
		{
			var request = new HttpRequestMessage(new HttpMethod(method), url);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			if(!string.IsNullOrEmpty(accessToken))
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			if (contentBody != null)
				request.Content = new StringContent(JsonConvert.SerializeObject(contentBody), Encoding.UTF8, "application/json");

			return request;
		}
	}
}
