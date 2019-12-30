using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMerchantService _merchantService;

        public MerchantsController(IMapper mapper, IMerchantService merchantService)
        {
            _mapper = mapper;
            _merchantService = merchantService;
        }

        [HttpGet]
        public async Task<ActionResult<Dto.Merchant[]>> GetList()
        {
            var userId = User.GetUserId();

            var list = await this._merchantService.GetListWithAccessCheckAsync(userId);

            return _mapper.Map<Dto.Merchant[]>(list);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.Merchant[]>> Update(int id, Dto.Merchant merchant)
        {
            var userId = User.GetUserId();

            var existMerchant = _mapper.Map<Models.Merchant>(merchant);

            var model = await _merchantService.EditMerchant(userId, existMerchant);

            return _mapper.Map<Dto.Merchant[]>(model);
        }
    }
}
