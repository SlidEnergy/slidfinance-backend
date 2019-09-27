using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
    public class EfRefreshTokensRepository : IRefreshTokensRepository
    {
        protected readonly ApplicationDbContext _dbContext;

        public EfRefreshTokensRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken> Find(string userId, string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.User.Id == userId && x.Token == token);
        }

		public async Task<RefreshToken> Add(RefreshToken entity)
		{
			_dbContext.Set<RefreshToken>().Add(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<RefreshToken> Update(RefreshToken entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
