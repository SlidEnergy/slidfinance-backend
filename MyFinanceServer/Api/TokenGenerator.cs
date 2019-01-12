using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFinanceServer.Api
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AppSettings _appSettings;

        public TokenGenerator(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string Get(string username)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = GetIdentity(username),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return serializedToken;
        }

        private ClaimsIdentity GetIdentity(string username)
        {
            return new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, username),
                });
        }
    }
}
