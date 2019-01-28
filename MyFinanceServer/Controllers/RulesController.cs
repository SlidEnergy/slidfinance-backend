using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api.Dto;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RulesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users/current
        [HttpGet("generated")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Dto.GeneratedRule>>> GetGeneratedRules()
        {
            var userId = User.GetUserId();

            var transactions = await _context.Transactions.AsNoTracking()
                .Where(x => x.Account.Bank.User.Id == userId)
                .GroupBy(x => new {AccountId = x.Account.Id, x.BankCategory, x.Description, x.Mcc })
                .Select(x => new GeneratedRule
                {
                    AccountId = x.Key.AccountId,
                    BankCategory = x.Key.BankCategory,
                    Description = x.Key.Description,
                    Mcc = x.Key.Mcc,
                    Categories = x.Where(s => s.Category != null).Select(s => s.Category.Id)
                        .GroupBy(s => s)
                        .Select(s => new CategoryDistribution { CategoryId = s.Key, Count = s.Count(c => c != null)}).ToArray(),
                    Count = x.Count(c => c.Id != null)
                })
                .ToListAsync();

            return _mapper.Map<Dto.GeneratedRule[]>(transactions);
        }
    }
}
