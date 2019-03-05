using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFinanceServer.Core;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenController(TokenService tokenService, UserManager<ApplicationUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenInfo>> Refresh(string token, string refreshToken)
        {
            try
            {
                var tokens = await _tokenService.RefreshToken(token, refreshToken);

                return new TokenInfo() { Token = tokens.Token, RefreshToken = tokens.RefreshToken };
            }
            catch (SecurityTokenException)
            {
                return BadRequest();
            }
        }
    }
}
