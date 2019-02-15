using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyFinanceServer.Api
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BanksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Bank>>> GetList()
        {
            var userId = User.GetUserId();

            var banks = await _context.Banks
                .Where(x => x.User.Id == userId)
                .OrderBy(x => x.Title)
                .ToListAsync();

            return _mapper.Map<Dto.Bank[]>(banks);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Bank>> Add(AddBankBindingModel bank)
        {
            var userId = User.GetUserId();

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Unauthorized();

            var newBank = user.LinkBank(bank.Title);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetList", _mapper.Map<Dto.Bank>(newBank));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EditBankBindingModel bank)
        {
            var userId = User.GetUserId();

            var editBank = await _context.Banks.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == userId);

            editBank.Rename(bank.Title);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            var bank = await _context.Banks.Include(x => x.User).FirstOrDefaultAsync(b => b.Id == id && b.User.Id == userId);

            if (bank == null)
                return NotFound();

            bank.User.UnlinkBank(id);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
