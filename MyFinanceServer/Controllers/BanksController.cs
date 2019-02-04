using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

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

        // GET: api/Banks
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Bank>>> GetBanks()
        {
            var userId = User.GetUserId();

            return await _context.Banks.Include(x=>x.Accounts)
                .Where(x => x.User.Id == userId)
                .Select(x => _mapper.Map<Dto.Bank>(x))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Bank>> AddBank(AddBankBindingModel bank)
        {
            var userId = User.GetUserId();

            var user = await _context.Users
                .Include(x => x.Banks)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Unauthorized();

            var newBank = user.LinkBank(bank.Title);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBanks", new { id = newBank.Id }, _mapper.Map<Dto.Bank>(newBank));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Bank>> DeleteBank(string id)
        {
            var userId = User.GetUserId();

            var user = await _context.Users
               .Include(x => x.Banks)
               .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Unauthorized();

            var bank = user.UnlinkBank(id);
            await _context.SaveChangesAsync();

            return _mapper.Map<Dto.Bank>(bank);
        }
    }
}
