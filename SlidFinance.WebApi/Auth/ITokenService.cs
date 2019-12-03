﻿using System.Threading.Tasks;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.WebApi
{
	public interface ITokenService
	{
		Task<TokensCortage> GenerateAccessAndRefreshTokens(ApplicationUser user, AccessMode accessMode);
		Task<TokensCortage> RefreshToken(string token, string refreshToken);
	}
}