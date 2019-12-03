using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface ITokenService
	{
		Task<TokensCortage> GenerateAccessAndRefreshTokens(ApplicationUser user, AccessMode accessMode);
		Task<TokensCortage> RefreshToken(string token, string refreshToken);
		Task AddToken(string userId, string token, AuthTokenType type);
	}
}