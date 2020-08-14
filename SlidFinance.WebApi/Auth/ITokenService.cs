using System.Threading.Tasks;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.WebApi
{
	public interface ITokenService
	{
		Task<TokensCortage> GenerateAccessAndRefreshTokens(ApplicationUser user, AccessMode accessMode);
		Task<string> GenerateToken(ApplicationUser user, AuthTokenType type);
		Task<TokensCortage> RefreshImportToken(string refreshToken);
		Task<TokensCortage> RefreshToken(string token, string refreshToken);
		Task<TokensCortage> CheckCredentialsAndGetToken(string email, string password);
	}
}