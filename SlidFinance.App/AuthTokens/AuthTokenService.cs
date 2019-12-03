using Microsoft.AspNetCore.Identity;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class AuthTokenService : IAuthTokenService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IAuthTokensRepository _repository;

		public AuthTokenService(UserManager<ApplicationUser> userManager, IAuthTokensRepository repository)
		{
			_repository = repository;
			_userManager = userManager;	
		}

		public async Task AddToken(string userId, string token, AuthTokenType type)
		{
			var user = await _userManager.FindByIdAsync(userId);

			var existToken = await _repository.FindAnyToken(token);

			if (existToken == null || existToken.Type != AuthTokenType.TelegramChatId || existToken.UserId != user.Id)
			{
				await _repository.Add(new AuthToken("any", token, user, type));
			}
		}

		public async Task<AuthToken> FindAnyToken(string token)
		{
			return await _repository.FindAnyToken(token);
		}

		public async Task<AuthToken> UpdateToken(AuthToken oldToken, string newToken)
		{
			oldToken.Update("any", newToken);
			await _repository.Update(oldToken);

			return oldToken;
		}
	}
}
