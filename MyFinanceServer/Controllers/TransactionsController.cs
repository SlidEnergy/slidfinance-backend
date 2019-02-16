using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<ActionResult<IEnumerable<Dto.Transaction>>> GetList()
        {
            var userId = User.GetUserId();

            return await _context.Transactions.Include(x=>x.Category).Include(x=>x.Account)
                .Where(x => x.Account.Bank.User.Id == userId).Select(x=>_mapper.Map<Dto.Transaction>(x)).ToListAsync();
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<Dto.Transaction> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

            var transaction = await _context.Transactions.Include(x=>x.Category)
                .SingleOrDefaultAsync(x => x.Id == id && x.Account.Bank.User.Id == userId);

            if (transaction == null)
                return NotFound();

            var dto = _mapper.Map<Dto.Transaction>(transaction);
            patchDoc.ApplyTo(dto);

            _mapper.Map(dto, transaction);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Dto.BankAccount>> Add(Dto.Transaction transaction)
        {
            var userId = User.GetUserId();

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == transaction.CategoryId && x.User.Id == userId);

            if (category == null)
                return NotFound();

            var account = await _context.Accounts
                    .FirstOrDefaultAsync(x => x.Id == transaction.AccountId && x.Bank.User.Id == userId);

            var newTransaction = _mapper.Map<Transaction>(transaction);

            _context.Transactions.Add(newTransaction);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetList", _mapper.Map<Dto.Transaction>(newTransaction));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Transaction>> Delete(int id)
        {
            var userId = User.GetUserId();

            var t = await _context.Transactions.FirstOrDefaultAsync(x => x.Account.Bank.User.Id == userId && x.Id == id);
            _context.Transactions.Remove(t);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
