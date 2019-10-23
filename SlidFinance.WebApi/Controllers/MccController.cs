using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Route("api/v1/[controller]")]
    [ApiController]
    public class MccController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMccService _mccService;

        public MccController(IMapper mapper, IMccService mccService)
        {
            _mapper = mapper;
            _mccService = mccService;
        }

        [HttpGet]
        public async Task<ActionResult<Mcc[]>> GetList()
        {
            var mcc = await this._mccService.GetList();

            return _mapper.Map<Mcc[]>(mcc);
        }
    }
}
