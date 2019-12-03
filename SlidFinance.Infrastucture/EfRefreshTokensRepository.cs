using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
    public class EfAuthTokenRepository : IAuthTokenRepository
    {
        protected readonly ApplicationDbContext _dbContext;

        public EfAuthTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AuthToken> FindRefreshToken(string userId, string token)
        {
            return await _dbContext.AuthTokens.FirstOrDefaultAsync(x => x.User.Id == userId && x.Token == token && x.Type == AuthTokenType.RefreshToken);
        }

		public async Task<AuthToken> FindAnyToken(string token)
		{
			return await _dbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token);
		}

		public async Task<AuthToken> Add(AuthToken entity)
		{
			_dbContext.Set<AuthToken>().Add(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<AuthToken> Update(AuthToken entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
