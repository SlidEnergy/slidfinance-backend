using SlidFinance.Domain;
using System.Collections.Generic;
using System.Security.Claims;

namespace SlidFinance.App
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}
