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
	public class ClaimsGenerator : IClaimsGenerator
	{
		private readonly IdentityOptions _identityOptions;

		public ClaimsGenerator(IOptions<IdentityOptions> identityOptions)
		{
			_identityOptions = identityOptions?.Value ?? new IdentityOptions();
		}

		public IEnumerable<Claim> CreateClaims(ApplicationUser user, AccessMode accessMode)
		{
			return new Claim[]
				{
					// sub, имя ClaimType берется из настроек в startup
					new Claim(_identityOptions.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
					// email, имя ClaimType берется из настроек в startup
					new Claim(_identityOptions.ClaimsIdentity.UserNameClaimType, user.UserName),

					// JWT specification

					// IssuedAt
					new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64),

					// Application

					new Claim(nameof(AccessMode), accessMode.ToString())
				};
		}

		/// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
		private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() -
								 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);
	}
}
