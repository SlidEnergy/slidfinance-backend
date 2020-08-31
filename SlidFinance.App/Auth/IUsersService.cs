using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IUsersService
	{
		Task<IdentityResult> CreateAccount(ApplicationUser user, string password);
		Task<ApplicationUser> GetById(string userId);

		Task<bool> IsAdmin(ApplicationUser user);

		Task<List<ApplicationUser>> GetListAsync();

		Task<ApplicationUser> GetByTelegramChatIdAsync(long chatId);

		Task<ApplicationUser> GetByApiKeyAsync(string apiKey);
	}
}