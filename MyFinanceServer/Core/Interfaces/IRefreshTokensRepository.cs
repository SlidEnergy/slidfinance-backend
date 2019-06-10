using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> GetByUserId(string userId);

		Task<RefreshToken> Add(RefreshToken entity);

		Task<RefreshToken> Update(RefreshToken entity);
    }
}
