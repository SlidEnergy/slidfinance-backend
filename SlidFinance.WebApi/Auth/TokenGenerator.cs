﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SlidFinance.App;
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

		public TokenGenerator(AuthSettings appSettings)
		{
			_authSettings = appSettings;
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
				return Convert.ToBase64String(randomNumber);
			}
		}

		/// <summary>
		/// Формирует AccessToken для указанного пользователя.
		/// </summary>
		public string GenerateAccessToken(ApplicationUser user, AccessMode accessMode) => GenerateAccessToken(CreateClaims(user, accessMode));

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

		private IEnumerable<Claim> CreateClaims(ApplicationUser user, AccessMode accessMode)
		{
			return new Claim[]
				{
					// Asp.net core Identity восстанавливает это значение в поле User.Identity.Name
					// Допустипо ClaimTypes.Name или ClaimsIdentity.DefaultNameClaimType
					// В startup.cs мы меняем UserNameClaimsType на "email"
					new Claim(JwtRegisteredClaimNames.Email, user.Email),

					// JWT specification

					// Asp.net core Identity использует claims "sub", как альтернативу ClaimTypes.NameIdentifier
					new Claim(JwtRegisteredClaimNames.Sub, user.Id),
					// IssuedAt
					new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64),

					// Application
					new Claim(nameof(AccessMode), accessMode.ToString())
				};
		}

		/// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
		private long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() -
								 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);
	}
}
