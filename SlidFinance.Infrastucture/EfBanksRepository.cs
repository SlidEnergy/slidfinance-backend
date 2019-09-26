using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.Infrastructure
{
    public class EfBanksRepository : EfRepository<Bank, int>, IRepositoryWithAccessCheck<Bank>
    {
        public EfBanksRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<List<Bank>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Banks.Where(x => x.User.Id == userId).OrderBy(x => x.Title).ToListAsync();
        }
    }
}
