using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Authorize(Policy = Policy.MustBeAllAccessMode)]
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
        public async Task<ActionResult<Dto.Bank[]>> GetList()
        {
            var userId = User.GetUserId();

            var banks = await _banksService.GetListWithAccessCheckAsync(userId);
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

            var editedBank = await _banksService.EditBank(userId, id, bank.Title);
            return _mapper.Map<Dto.Bank>(editedBank);
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
