using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class TransactionRepository
    {
        private ApplicationDbContext _dbContext;

        public TransactionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(int accountId, Models.Transaction transaction)
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            account.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.Transaction>> GetList()
        {
            return await _dbContext.Transactions.ToListAsync();
        }
    }
}
