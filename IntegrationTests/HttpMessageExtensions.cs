using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
    public static class HttpMessageExtensions
    {
        public static async Task<Dictionary<string, object>> ToDictionary(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public static async Task<object[]> ToArray(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<object[]>(json);
        }

        public static async Task<Dictionary<string, object>[]> ToArrayOfDictionaries(this HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>[]>(json);
        }
    }
}
