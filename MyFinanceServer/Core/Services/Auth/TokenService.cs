using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFinanceServer.Data;
using MyFinanceServer.Shared;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class TokenService
    {
        private readonly EfRefreshTokensRepository _repository;
        private readonly ITokenGenerator _tokenGenerator;

        public TokenService(EfRefreshTokensRepository repository, ITokenGenerator tokenGenerator)
        {
            _repository = repository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<TokensCortage> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var userId = principal.GetUserId();

            var savedRefreshToken = await GetRefreshToken(userId);
            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newToken = _tokenGenerator.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            await UpdateRefreshToken(userId, newRefreshToken);

            return new TokensCortage() { Token = newToken, RefreshToken = newRefreshToken };
        }

        private async Task<string> GetRefreshToken(string userId) => (await _repository.GetByUserId(userId)).Token;

        private async Task UpdateRefreshToken(string userId, string refreshToken)
        {
            var token = await _repository.GetByUserId(userId);
            token.Token = refreshToken;
            token.ExpirationDate = DateTime.Now.AddMonths(6);
            token.DeviceId = "any";
            await _repository.Update(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the server key used to sign the JWT token is here, use more than 16 chars")),
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
