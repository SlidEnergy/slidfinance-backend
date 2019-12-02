using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public interface IAuthTokenRepository
    {
        Task<AuthToken> FindRefreshToken(string userId, string token);

		Task<AuthToken> Add(AuthToken entity);

		Task<AuthToken> Update(AuthToken entity);
    }
}
