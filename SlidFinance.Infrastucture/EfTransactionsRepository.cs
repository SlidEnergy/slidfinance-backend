using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.Infrastucture
{
    public class EfTransactionsRepository : EfRepository<Transaction, int>, IRepositoryWithAccessCheck<Transaction>
    {
        public EfTransactionsRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<List<Transaction>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Transactions.Where(x => x.Account.Bank.User.Id == userId).ToListAsync();
        }
    }
}
