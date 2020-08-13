using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.Auth
{
	public class ApiKeyProvider : IApiKeyProvider
	{
		private readonly ILogger<ApiKeyProvider> _logger;
		private readonly IUsersService _usersService;

		public ApiKeyProvider(ILogger<ApiKeyProvider> logger, IUsersService usersService)
		{
			_logger = logger;
			_usersService = usersService;
		}

		public async Task<IApiKey> ProvideAsync(string key)
		{
			try
			{
				if (string.IsNullOrEmpty(key))
					return null;

				ApplicationUser user = await _usersService.GetByApiKeyAsync(key);

				if (user == null)
					return null;

				var apiKey = new ApiKey("TESTAPIKEY=", user.Id, new List<Claim>
				{
					new Claim(nameof(AccessMode), AccessMode.Export.ToString())
				});


				return await Task.FromResult(apiKey);
			}
			catch (System.Exception exception)
			{
				_logger.LogError(exception, exception.Message);
				throw;
			}
		}
	}
}
