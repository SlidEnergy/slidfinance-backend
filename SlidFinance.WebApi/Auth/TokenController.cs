using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SlidFinance.App;
using SlidFinance.WebApi.Dto;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Route("api/v1/[controller]")]
    [ApiController]
    public sealed class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenInfo>> Refresh(TokensCortage tokens)
        {
            try
            {
                var newTokens = await _tokenService.RefreshToken(tokens.Token, tokens.RefreshToken);

				return new TokenInfo() { Token = newTokens.Token, RefreshToken = newTokens.RefreshToken };
            }
            catch (SecurityTokenException)
            {
                return BadRequest();
            }
        }
    }
}
