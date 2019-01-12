using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Transaction>>> GetTransaction()
        {
            var list = await _transactionRepository.GetList();

            return Ok(list);
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(TransactionBindingModel transaction)
        {
            var t = new Models.Transaction()
            {
                Amount = transaction.Amount,
                DateTime = transaction.DateTime,
                Category = Category.None,
                Description = transaction.Category + ":" + transaction.Description
            };

            await _transactionRepository.Add(t);

            return CreatedAtAction("GetTransaction", new { id = t.Id }, transaction);
        }
    }
}
