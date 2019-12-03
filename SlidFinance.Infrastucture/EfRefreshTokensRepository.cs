using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
    public class EfAuthTokenRepository: EfRepository<AuthToken, int>, IAuthTokenRepository
	{
        public EfAuthTokenRepository(ApplicationDbContext dbContext) : base(dbContext) { }

		public async Task<AuthToken> FindAnyToken(string token)
		{
			return await _dbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token);
		}
    }
}
