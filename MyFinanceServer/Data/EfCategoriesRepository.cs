using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class EfCategoriesRepository : EfRepository, ICategoriesRepository
    {
        public EfCategoriesRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<Category> GetById(int id) => await GetById<int, Category>(id);

        public async Task<List<Category>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Categories.Where(x=>x.User.Id == userId).ToListAsync();
        }
    }
}
