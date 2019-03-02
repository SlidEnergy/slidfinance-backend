using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface ICategoriesRepository : IRepository
    {
        Task<Category> GetById(int id);

        Task<List<Category>> GetListWithAccessCheck(string userId);
    }
}
