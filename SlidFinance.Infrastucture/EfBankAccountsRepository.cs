using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using SlidFinance.App;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.Infrastucture
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
