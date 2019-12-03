using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Security.Claims;

namespace SlidFinance.WebApi
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(ApplicationUser user, AccessMode accessMode);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}
