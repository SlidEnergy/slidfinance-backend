using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly RulesService _rulesService;

        public RulesController(IMapper mapper, RulesService rulesService)
        {
            _mapper = mapper;
            _rulesService = rulesService;
        }

        [HttpGet]
        public async Task<ActionResult<Dto.Rule[]>> GetList()
        {
            var userId = User.GetUserId();

            var rules = await this._rulesService.GetList(userId);

            return _mapper.Map<Dto.Rule[]>(rules);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Rule>> Add(Dto.Rule rule)
        {
            var userId = User.GetUserId();

            var newRule = await _rulesService.AddRule(userId, rule.AccountId, rule.BankCategory, rule.CategoryId, rule.Description, rule.Mcc);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Rule>(newRule));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.Rule>> Update(int id, Dto.Rule rule)
        {
            var userId = User.GetUserId();

            var editedRule = await _rulesService.EditRule(userId, id, rule.AccountId, rule.BankCategory, rule.CategoryId, rule.Description, rule.Mcc);
            return _mapper.Map<Dto.Rule>(editedRule);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Rule>> Delete(int id)
        {
            var userId = User.GetUserId();

            await _rulesService.DeleteRule(userId, id);

            return NoContent();
        }

        [HttpGet("generated")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<GeneratedRule>>> GetGeneratedRules()
        {
            var userId = User.GetUserId();

            var rules = await _rulesService.GenerateRules(userId);

            return rules;
        }
    }
}
