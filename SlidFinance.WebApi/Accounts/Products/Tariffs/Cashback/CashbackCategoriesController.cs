using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Route("api/v1")]
    [Authorize(Policy = Policy.MustBeAllAccessMode)]
    [ApiController]
    public sealed class CashbackCategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICashbackCategoriesService _service;

        public CashbackCategoriesController(IMapper mapper, ICashbackCategoriesService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet("tariffs/{tariffId}/cashback/categories")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<CashbackCategory>>> GetList(int tariffId)
        {
            var userId = User.GetUserId();

            var products = await _service.GetListWithAccessCheckAsync(userId, tariffId);

            return products;
        }
    }
}
