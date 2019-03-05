using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class EfRefreshTokensRepository : EfRepository
    {
        public EfRefreshTokensRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<RefreshToken> GetByUserId(string userId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.User.Id == userId);
        }
    }
}
