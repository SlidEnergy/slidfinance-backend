using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TransactionsController(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Transaction>>> GetTransactions()
        {
            var userId = User.GetUserId();

            return await _context.Transactions.Include(x=>x.Category).Include(x=>x.Account)
                .Where(x => x.Account.Bank.User.Id == userId).Select(x=>_mapper.Map<Dto.Transaction>(x)).ToListAsync();
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PatchTransaction(string id, PatchTransactionBindingModel transactionData)
        {
            var userId = User.GetUserId();

            var transaction =
                await _context.Transactions.SingleOrDefaultAsync(x => x.Id == id && x.Account.Bank.User.Id == userId);

            if (transaction == null)
                return NotFound();

            if (string.IsNullOrEmpty(transactionData.CategoryId))
            {
                transaction.Category = null;
            }
            else
            {
                var category = await _context.Categories.FindAsync(transactionData.CategoryId);

                if (category == null)
                    return NotFound();

                transaction.Category = category;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
