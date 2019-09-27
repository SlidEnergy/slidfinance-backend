using NUnit.Framework;
using System.Net;
using System.Net.Http;

namespace SlidFinance.WebApi.IntegrationTests
{
	/// <summary>
	/// Методы расширения для класса HttpResponseMessage проверяющие статус ответа.
	/// </summary>
	public static class HttpResponseMessageAssertExtensions
	{
		/// <summary>
		/// Проверяет содержит ли ответ статус BadRequest.
		/// </summary>
		public static void IsBadRequest(this HttpResponseMessage response)
		{
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}

		/// <summary>
		/// Проверяет содержит ли ответ статус NotFound.
		/// </summary>
		public static void IsNotFound(this HttpResponseMessage response)
		{
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}

		/// <summary>
		/// Проверяет содержит ли ответ статус Unauthorized.
		/// </summary>
		public static void IsUnauthorized(this HttpResponseMessage response)
		{
			Assert.IsNotNull(response);
			Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		/// <summary>
		/// Проверяет содержит ли ответ успешный статус.
		/// </summary>
		public static void IsSuccess(this HttpResponseMessage response)
		{
			Assert.IsNotNull(response);
			Assert.IsTrue(response.IsSuccessStatusCode);
		}
	}
}
