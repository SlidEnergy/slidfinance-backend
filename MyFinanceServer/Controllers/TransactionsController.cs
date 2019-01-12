using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public sealed class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Transaction>>> GetTransaction()
        {
            throw new NotImplementedException();
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction(TransactionBindingModel transaction)
        {
            var t = new Models.Transaction()
            {
                Amount = transaction.Amount,
                DateTime = transaction.DateTime,
                Category = Category.None,
                Description = transaction.Category + " : " + transaction.Description
            };

            var account = await _dbContext.Accounts
                .Include(x => x.Transactions)
                .SingleOrDefaultAsync(x=> x.Id == transaction.AccountId);

            if (account == null)
                NotFound();

            account.Transactions.Add(t);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = t.Id }, t);
        }
    }
}
