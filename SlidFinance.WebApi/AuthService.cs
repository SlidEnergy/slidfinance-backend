using Microsoft.AspNetCore.Identity;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;

		public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
		{
			_tokenService = tokenService;
			_userManager = userManager;
		}

		public async Task<TokensCortage> CheckCredentialsAndGetToken(string email, string password)
		{
			var user = await _userManager.FindByNameAsync(email);

			if (user == null)
				throw new AuthenticationException();

			var checkResult = await _userManager.CheckPasswordAsync(user, password);

			if (!checkResult)
				throw new AuthenticationException();

			return await _tokenService.GenerateAccessAndRefreshTokens(user, AccessMode.All);
		}
	}
}
