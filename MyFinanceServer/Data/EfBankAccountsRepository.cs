using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class EfBankAccountsRepository : EfRepository<BankAccount, int>, IRepositoryWithAccessCheck<BankAccount>
    {
        public EfBankAccountsRepository(ApplicationDbContext dbContext) : base(dbContext) {}

        public async Task<List<BankAccount>> GetListWithAccessCheck(string userId)
        {
            return await _dbContext.Accounts.Where(x => x.Bank.User.Id == userId).OrderBy(x => x.Title).ToListAsync();
        }
    }
}
