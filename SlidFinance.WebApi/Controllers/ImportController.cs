using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IMapper _mapper;
        ImportService _service;

        public ImportController(IMapper mapper, ImportService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> Import(PatchAccountDataBindingModel data)
        {
            var userId = User.GetUserId();

            var count = await _service.Import(userId, data.Code, data.Balance, _mapper.Map<Transaction[]>(data.Transactions));

            return count;
        }
    }
}
