using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> Find(string userId, string token);

		Task<RefreshToken> Add(RefreshToken entity);

		Task<RefreshToken> Update(RefreshToken entity);
    }
}
