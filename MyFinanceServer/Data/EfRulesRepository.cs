using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class EfRulesRepository : EfRepository<Rule, int>, IRepositoryWithAccessCheck<Rule>
    {
        public EfRulesRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<List<Rule>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Rules.Where(x => x.Account.Bank.User.Id == userId).OrderBy(x => x.Order).ToListAsync();
        }
    }
}
