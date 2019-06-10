using Microsoft.AspNetCore.Identity;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
	public class UsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenGenerator _tokenGenerator;
		private readonly TokenService _tokenService;

		public UsersService(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator, TokenService tokenService)
        {
			_tokenService = tokenService;
			_userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ApplicationUser> GetById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> Register(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<TokensCortage> Login(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
                throw new AuthenticationException();

            var checkResult = await _userManager.CheckPasswordAsync(user, password);

            if (!checkResult)
                throw new AuthenticationException();

			var refreshToken = _tokenGenerator.GenerateRefreshToken();
			await _tokenService.AddRefreshToken(refreshToken, user);

            return new TokensCortage()
            {
                Token = _tokenGenerator.GenerateAccessToken(user),
                RefreshToken = refreshToken
            };
        }
    }
}
