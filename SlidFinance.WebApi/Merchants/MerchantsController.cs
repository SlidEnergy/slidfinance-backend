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
        public async Task<ActionResult<Merchant[]>> GetList()
        {
            var userId = User.GetUserId();

            var list = await this._merchantService.GetListWithAccessCheckAsync(userId);

            return list.ToArray();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Merchant>> Update(int id, Merchant merchant)
        {
            var userId = User.GetUserId();

            var model = await _merchantService.EditMerchant(userId, merchant);

            return model;
        }
    }
}
