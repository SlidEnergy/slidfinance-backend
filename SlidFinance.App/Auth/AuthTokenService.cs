using Microsoft.AspNetCore.Identity;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class AuthTokenService : IAuthTokenService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly DataAccessLayer _dal;

		public AuthTokenService(UserManager<ApplicationUser> userManager, DataAccessLayer dal)
		{
			_dal = dal;
			_userManager = userManager;	
		}

		public async Task AddToken(string userId, string token, AuthTokenType type)
		{
			var user = await _userManager.FindByIdAsync(userId);

			var existToken = await _dal.AuthTokens.FindAnyToken(token);

			if (existToken == null || existToken.Type != AuthTokenType.TelegramChatId || existToken.UserId != user.Id)
			{
				await _dal.AuthTokens.Add(new AuthToken("any", token, user, type));
			}
		}

		public async Task<AuthToken> FindAnyToken(string token)
		{
			return await _dal.AuthTokens.FindAnyToken(token);
		}

		public async Task<AuthToken> UpdateToken(AuthToken oldToken, string newToken)
		{
			oldToken.Update("any", newToken);
			await _dal.AuthTokens.Update(oldToken);

			return oldToken;
		}
	}
}
