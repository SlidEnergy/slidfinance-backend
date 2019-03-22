using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IBanksRepository : IRepository
    {
        Task<Bank> GetById(int id);

        Task<List<Bank>> GetListWithAccessCheck(string userId);
    }
}
