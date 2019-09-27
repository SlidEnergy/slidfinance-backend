using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IUsersService
	{
		Task<TokensCortage> CheckCredentialsAndGetToken(string email, string password);
		Task<IdentityResult> CreateAccount(ApplicationUser user, string password);
		Task<ApplicationUser> GetById(string userId);
	}
}