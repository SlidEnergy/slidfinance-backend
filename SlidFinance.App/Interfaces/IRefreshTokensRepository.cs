using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> GetByUserId(string userId);

		Task<RefreshToken> Add(RefreshToken entity);

		Task<RefreshToken> Update(RefreshToken entity);
    }
}
