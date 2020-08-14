using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SlidFinance.App;
using SlidFinance.App.Utils;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SlidFinance.WebApi
{
	/// <summary>
	/// Формирует AccessToken и RefreshToken, необходимые для авторизации.
	/// </summary>
	public class TokenGenerator : ITokenGenerator
	{
		private readonly AuthSettings _authSettings;
		private readonly IClaimsGenerator _claimsGenerator;

		public TokenGenerator(AuthSettings appSettings, IClaimsGenerator claimsGenerator)
		{
			_authSettings = appSettings;
			_claimsGenerator = claimsGenerator;
		}

		/// <summary>
		/// Формирует RefreshToken, нужный для получения нового AccessToken после его истечения
		/// </summary>
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return StringUtils.ToBase64StringWithUrlAndFilenameSafe(randomNumber);
			}
		}

		/// <summary>
		/// Формирует AccessToken для указанного пользователя.
		/// </summary>
		public string GenerateAccessToken(ApplicationUser user, AccessMode accessMode) => GenerateAccessToken(_claimsGenerator.CreateClaims(user, accessMode));

		/// <summary>
		/// Формирует AccessToken с указанными Claims.
		/// </summary>
		public string GenerateAccessToken(IEnumerable<Claim> claims)
		{
			var token = new JwtSecurityToken(
				issuer: _authSettings.Issuer,
				audience: _authSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_authSettings.LifetimeMinutes),
				signingCredentials: new SigningCredentials(_authSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

			var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

			return encodedToken;
		}
	}
}
