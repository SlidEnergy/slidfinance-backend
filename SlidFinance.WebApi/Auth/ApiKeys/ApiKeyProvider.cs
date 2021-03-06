﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.Auth
{
	public class ApiKeyProvider : IApiKeyProvider
	{
		private readonly ILogger<ApiKeyProvider> _logger;
		private readonly IUsersService _usersService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IClaimsGenerator _claimsGenerator;

		public ApiKeyProvider(ILogger<ApiKeyProvider> logger, IUsersService usersService, UserManager<ApplicationUser> userManager, IClaimsGenerator claimsGenerator)
		{
			_logger = logger;
			_usersService = usersService;
			_userManager = userManager;
			_claimsGenerator = claimsGenerator;
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

				var roles = await _userManager.GetRolesAsync(user);

				var claims = _claimsGenerator.CreateClaims(user, roles, AccessMode.Export);

				var apiKey = new ApiKey("TESTAPIKEY=", user.Id, claims);

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
