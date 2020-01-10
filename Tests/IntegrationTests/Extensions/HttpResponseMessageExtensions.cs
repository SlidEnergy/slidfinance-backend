using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	/// <summary>
	/// Методы расширения для класса HttpResponseMessage представляющие содержимое ответа.
	/// </summary>
	public static class HttpResponseMessageExtensions
    {
		/// <summary>
		/// Представляет содержимое ответа как словарь ключ-значение
		/// </summary>
        public static async Task<Dictionary<string, object>> ToDictionary(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

		/// <summary>
		/// Представляет содержимое ответа как массив объектов.
		/// </summary>
        public static async Task<object[]> ToArray(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<object[]>(json);
        }

		/// <summary>
		/// Представляет содержимое ответа как массив словарей ключ-значение.
		/// </summary>
        public static async Task<Dictionary<string, object>[]> ToArrayOfDictionaries(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>[]>(json);
        }

		/// <summary>
		/// Представляет содержимое ответа как объект.
		/// </summary>
		public static async Task<T> ToObject<T>(this HttpResponseMessage response)
		{
			string json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static async Task<string> ToJsonString(this HttpResponseMessage response)
		{
			string json = await response.Content.ReadAsStringAsync();
			return (string)JsonConvert.DeserializeObject(json);
		}
	}
}
