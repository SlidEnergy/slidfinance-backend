using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using MyFinanceServer.Models;

namespace MyFinanceServer.Api
{
    [Route("auth/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        ITokenGenerator _tokenGenerator;
        IUserRepository _userRepository;

        public AuthController(ITokenGenerator tokenGenerator, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody]User userData)
        {
            var user = _userRepository.Get(userData.Email, userData.Password);
            if (user == null)
                return Unauthorized();

            var token = _tokenGenerator.Get(userData.Email);

            return Ok(new { token, userData.Email });
        }
    }
}
