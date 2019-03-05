using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IRefreshTokensRepository : IRepository
    {
        Task<RefreshToken> GetByUserId(string userId);
    }
}
