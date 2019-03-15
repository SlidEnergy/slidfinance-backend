using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyFinanceServer.Core;
using MyFinanceServer.Shared;

namespace MyFinanceServer.Api
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BanksService _banksService;

        public BanksController(ApplicationDbContext context, IMapper mapper, BanksService banksService)
        {
            _context = context;
            _mapper = mapper;
            _banksService = banksService;
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

            var newBank = await _banksService.AddBank(userId, bank.Title);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Bank>(newBank));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.Bank>> Update(int id, EditBankBindingModel bank)
        {
            var userId = User.GetUserId();

            var editBank = await _context.Banks.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == userId);

            editBank.Rename(bank.Title);

            await _context.SaveChangesAsync();

            return _mapper.Map<Dto.Bank>(editBank);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _banksService.DeleteBank(userId, id);

            return NoContent();
        }
    }
}
