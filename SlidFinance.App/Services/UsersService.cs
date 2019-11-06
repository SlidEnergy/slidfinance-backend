using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class UsersService : IUsersService
	{
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;

		public UsersService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
			_tokenService = tokenService;
			_userManager = userManager;
        }

		public async Task<List<ApplicationUser>> GetListAsync()
		{
			return await _userManager.Users.ToListAsync();
		}

		public async Task<ApplicationUser> GetById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> CreateAccount(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<TokensCortage> CheckCredentialsAndGetToken(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
                throw new AuthenticationException();

            var checkResult = await _userManager.CheckPasswordAsync(user, password);

            if (!checkResult)
                throw new AuthenticationException();

			return await _tokenService.GenerateAccessAndRefreshTokens(user, AccessMode.All);
        }
	}
}
