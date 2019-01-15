using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System.Linq;

namespace MyFinanceServer.Api
{
    [Route("auth/v1")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ApplicationDbContext _dbContext;

        public AuthController(ITokenGenerator tokenGenerator, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<TokenInfo> GetToken(UserBindingModel userData)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Email == userData.Email && x.Password == userData.Password);
            
            if (user == null)
                return Unauthorized();

            var token = _tokenGenerator.Get(user);

            return new TokenInfo() { Token = token, Email = userData.Email };
        }
    }
}
