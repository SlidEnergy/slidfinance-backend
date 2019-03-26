using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
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
