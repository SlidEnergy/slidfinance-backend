using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using System.Linq;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class TokensController : ControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _dbContext;

        public TokensController(ITokenGenerator tokenGenerator, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<TokenInfo> CreateToken(UserBindingModel userData)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Email == userData.Email && x.Password == userData.Password);
            
            if (user == null)
                return Unauthorized();

            var token = _tokenGenerator.Get(user);

            return new TokenInfo() { Token = token, Email = userData.Email };
        }
    }
}
