using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.Auth
{
	public class ApiKeyProvider : IApiKeyProvider
	{
		private readonly ILogger<ApiKeyProvider> _logger;

		public ApiKeyProvider(ILogger<ApiKeyProvider> logger)
		{
			_logger = logger;
		}

		public async Task<IApiKey> ProvideAsync(string key)
		{
			try
			{
				if (string.IsNullOrEmpty(key))
					return null;

				ApplicationUser user = null;

				if (user == null)
					return null;

				return await Task.FromResult(new ApiKey("TESTAPIKEY=", user.Id));
			}
			catch (System.Exception exception)
			{
				_logger.LogError(exception, exception.Message);
				throw;
			}
		}
	}
}
