using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System.Collections.Generic;
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
		public async Task<ActionResult<Dto.Bank[]>> GetList()
        {
            var banks = await _banksService.GetLis();
            return _mapper.Map<Dto.Bank[]>(banks);
        }

        [HttpPost]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult<Dto.Bank>> Add(AddBankBindingModel bank)
        {
            var newBank = await _banksService.AddBank(bank.Title);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Bank>(newBank));
        }

        [HttpPut("{id}")]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult<Dto.Bank>> Update(int id, EditBankBindingModel bank)
        {
            var editedBank = await _banksService.EditBank(id, bank.Title);
            return _mapper.Map<Dto.Bank>(editedBank);
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
