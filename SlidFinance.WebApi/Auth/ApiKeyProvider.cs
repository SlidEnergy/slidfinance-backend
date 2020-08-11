using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;

namespace SlidFinance.WebApi.Auth
{
	public class ApiKeyProvider : IApiKeyProvider
	{
		private readonly ILogger<ApiKeyProvider> _logger;

		public ApiKeyProvider(ILogger<ApiKeyProvider> logger)
		{
			_logger = logger;
		}

		public Task<IApiKey> ProvideAsync(string key)
		{
			try
			{
				// write your validation implementation here and return an instance of a valid ApiKey or retun null for an invalid key.
				return Task.FromResult<IApiKey>(null);
			}
			catch (System.Exception exception)
			{
				_logger.LogError(exception, exception.Message);
				throw;
			}
		}
	}
}
