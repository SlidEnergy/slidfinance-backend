using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
    public class EfAuthTokensRepository: EfRepository<AuthToken, int>, IAuthTokensRepository
	{
        public EfAuthTokensRepository(ApplicationDbContext dbContext) : base(dbContext) { }

		public async Task<AuthToken> FindAnyToken(string token)
		{
			return await _dbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token);
		}
    }
}
