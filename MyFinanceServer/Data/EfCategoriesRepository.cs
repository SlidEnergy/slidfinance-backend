using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class EfCategoriesRepository : EfRepository<Category, int>, IRepositoryWithAccessCheck<Category>
    {
        public EfCategoriesRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<List<Category>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Categories.Where(x => x.User.Id == userId).OrderBy(x => x.Order).ToListAsync();
        }
    }
}
