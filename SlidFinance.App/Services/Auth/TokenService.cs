using Microsoft.IdentityModel.Tokens;
using SlidFinance.Domain;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class TokenService : ITokenService
	{
        private readonly IRefreshTokensRepository _repository;
        private readonly ITokenGenerator _tokenGenerator;
		private readonly AuthSettings _authSettings;

		public TokenService(IRefreshTokensRepository repository, ITokenGenerator tokenGenerator, AuthSettings authSettings)
        {
            _repository = repository;
            _tokenGenerator = tokenGenerator;
			_authSettings = authSettings;
		}

        public async Task<TokensCortage> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var userId = principal.GetUserId();
			var savedToken = await _repository.Find(userId, refreshToken);

            if (savedToken == null)
                throw new SecurityTokenException("Invalid refresh token");

            var newToken = _tokenGenerator.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

			savedToken.Update("any", newRefreshToken);
			await _repository.Update(savedToken);

			return new TokensCortage() { Token = newToken, RefreshToken = newRefreshToken };
        }

		public async Task<TokensCortage> GenerateAccessAndRefreshTokens(ApplicationUser user, AccessMode accessMode)
		{
			var refreshToken = _tokenGenerator.GenerateRefreshToken();
			await AddRefreshToken(refreshToken, user);

			return new TokensCortage()
			{
				Token = _tokenGenerator.GenerateAccessToken(user, accessMode),
				RefreshToken = refreshToken
			};
		}

		private async Task<RefreshToken> AddRefreshToken(string refreshToken, ApplicationUser user)
		{
			var token = new RefreshToken("any", refreshToken, user);
			await _repository.Add(token);
			return token;
		}

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
			var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _authSettings.GetSymmetricSecurityKey(),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
	}
}
