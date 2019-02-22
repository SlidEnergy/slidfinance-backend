using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class TokensController : ControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokensController(ITokenGenerator tokenGenerator, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<TokenInfo>> CreateToken(UserBindingModel userData)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByNameAsync(userData.Email);

            if (user == null)
                return NotFound();

            if (!(await _userManager.CheckPasswordAsync(user, userData.Password)))
                return Unauthorized();

            var token = _tokenGenerator.Get(user);

            return new TokenInfo() { Token = token, Email = userData.Email };
        }
    }
}
