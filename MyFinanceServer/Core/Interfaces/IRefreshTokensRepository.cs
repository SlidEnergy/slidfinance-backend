using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> GetByUserId(string userId);

        Task<RefreshToken> Update(RefreshToken entity);
    }
}
