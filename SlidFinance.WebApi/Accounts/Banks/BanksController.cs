using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBanksService _banksService;

        public BanksController(IMapper mapper, IBanksService banksService)
        {
            _mapper = mapper;
            _banksService = banksService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
		[Authorize(Policy = Policy.MustBeAllAccessMode)]
		public async Task<ActionResult<Bank[]>> GetList()
        {
            var banks = await _banksService.GetLis();
            return _mapper.Map<Bank[]>(banks);
        }

        [HttpPost]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult<Bank>> Add(Bank bank)
        {
            var newBank = await _banksService.AddBank(bank.Title);

            return CreatedAtAction("GetList", _mapper.Map<Bank>(newBank));
        }

        [HttpPut("{id}")]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult<Bank>> Update(int id, Bank bank)
        {
            var editedBank = await _banksService.EditBank(id, bank.Title);
            return _mapper.Map<Bank>(editedBank);
        }

        [HttpDelete("{id}")]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult> Delete(int id)
        {
            await _banksService.DeleteBank(id);

            return NoContent();
        }
    }
}
