using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyFinanceServer.Api.Dto;
using BankAccount = MyFinanceServer.Data.BankAccount;
using Rule = MyFinanceServer.Data.Rule;

namespace MyFinanceServer.Api
{
    [Route("api/[controller]")]
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

        // GET: api/Rules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dto.Rule>>> GetRule()
        {
            var userId = User.GetUserId();

            return await _context.Rules
                .Include(x=>x.Account)
                .Include(x=>x.Category)
                .OrderBy(x => x.Order)
                .Where(x=>x.Account.Bank.User.Id == userId)
                .Select(x=>_mapper.Map<Dto.Rule>(x)).ToListAsync();
        }

        // POST: api/Rules
        [HttpPost]
        public async Task<ActionResult<Dto.Rule>> PostRule(Dto.Rule rule)
        {
            var userId = User.GetUserId();

            BankAccount account = null;

            account = await _context.Accounts
                .FirstOrDefaultAsync(x => x.Bank.User.Id == userId && x.Id == rule.AccountId);

            if (account == null)
                return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(x => x.User.Id == userId && x.Id == rule.CategoryId);

            if (category == null)
                return NotFound();

            var order = await _context.Rules.MaxAsync(x => (int?) x.Order) ?? 0;
            order++;

            var newRule = new Rule
            {
                Account = account,
                BankCategory = rule.BankCategory,
                Category = category,
                Description = rule.Description,
                Mcc = rule.Mcc,
                Order = order
            };
            _context.Rules.Add(newRule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRule", new {id = newRule.Id}, _mapper.Map<Dto.Rule>(newRule));
        }

        // DELETE: api/Rules/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Rule>> DeleteRule(int id)
        {
            var userId = User.GetUserId();

            var rule = await _context.Rules.SingleOrDefaultAsync(x => x.Account.Bank.User.Id == userId && x.Id == id);
            if (rule == null)
            {
                return NotFound();
            }

            _context.Rules.Remove(rule);
            await _context.SaveChangesAsync();

            return _mapper.Map<Dto.Rule>(rule);
        }

        // GET: api/Users/current
        [HttpGet("generated")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Dto.GeneratedRule>>> GetGeneratedRules()
        {
            var userId = User.GetUserId();

            var generatedRules = await _context.Transactions
                .Where(x => x.Account.Bank.User.Id == userId && x.Category != null)
                .GroupBy(x => new { AccountId = x.Account.Id, x.BankCategory, x.Description, x.Mcc })
                .Select(x => new GeneratedRule
                {
                    AccountId = x.Key.AccountId,
                    BankCategory = x.Key.BankCategory,
                    Description = x.Key.Description,
                    Mcc = x.Key.Mcc,
                    Categories = x.Where(s => s.Category != null).Select(s => s.Category.Id)
                        .GroupBy(s => s)
                        .Select(s => new CategoryDistribution { CategoryId = s.Key, Count = s.Count() }).ToArray(),
                    Count = x.Count()
                })
                .Where(x => x.Count > 5)
                .ToListAsync();

            var rules = await _context.Rules
                .Where(x => x.Category.User.Id == userId)
                .Select(x => new Dto.GeneratedRule
                {
                    AccountId = x.Account.Id,
                    BankCategory = x.BankCategory,
                    Description = x.Description,
                    Mcc = x.Mcc,
                })
                .ToListAsync();

            generatedRules = generatedRules.Except(rules, new RuleComparer()).ToList();

            return _mapper.Map<Dto.GeneratedRule[]>(generatedRules);
        }
    }

    public class RuleComparer : IEqualityComparer<GeneratedRule>
    {
        bool IEqualityComparer<GeneratedRule>.Equals(GeneratedRule x, GeneratedRule y)
        {
            return ((x.AccountId == null || x.AccountId.Equals(y.AccountId)) &&
                    (string.IsNullOrEmpty(x.BankCategory) || x.BankCategory.Equals(y.BankCategory)) &&
                    (string.IsNullOrEmpty(x.Description) || x.Description.Equals(y.Description)) &&
                    (x.Mcc == null || x.Mcc.Equals(y.Mcc)));
        }

        int IEqualityComparer<GeneratedRule>.GetHashCode(GeneratedRule obj)
        {
            if (obj == null)
                return 0;

            return (obj.BankCategory ?? "").GetHashCode() * 1000 + 
                   (obj.Description ?? "").GetHashCode() * 100 +
                   ((obj.AccountId ?? 0) + 1) * 10 +
                   obj.Mcc ?? 0;
        }
    }
}
