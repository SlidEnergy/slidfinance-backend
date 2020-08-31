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
		private readonly DataAccessLayer _dal;

		public UsersService(UserManager<ApplicationUser> userManager, DataAccessLayer dal)
		{
			_userManager = userManager;
			_dal = dal;
		}

		public async Task<List<ApplicationUser>> GetListAsync()
		{
			return await _userManager.Users.ToListAsync();
		}

		public async Task<ApplicationUser> GetById(string userId)
		{
			return await _userManager.FindByIdAsync(userId);
		}

		public async Task<bool> IsAdmin(ApplicationUser user)
		{
			return await _userManager.IsInRoleAsync(user, Role.Admin);
		}

		public async Task<ApplicationUser> GetByTelegramChatIdAsync(long chatId)
		{
			var token = await _dal.AuthTokens.FindAnyToken(chatId.ToString());

			if (token != null && token.Type == AuthTokenType.TelegramChatId)
				return token.User;

			return null;
		}

		public async Task<ApplicationUser> GetByApiKeyAsync(string apiKey)
		{
			var token = await _dal.AuthTokens.FindAnyToken(apiKey);

			if (token != null && token.Type == AuthTokenType.ApiKey)
				return token.User;

			return null;
		}

		public async Task<IdentityResult> CreateAccount(ApplicationUser user, string password)
		{
			return await _userManager.CreateAsync(user, password);
		}
	}
}
