using System.Collections.Generic;
using System.Security.Claims;

namespace MyFinanceServer.Core
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}
