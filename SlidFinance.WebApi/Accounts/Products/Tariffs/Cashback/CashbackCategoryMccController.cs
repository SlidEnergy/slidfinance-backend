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
    public sealed class CashbackCategoryMccController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICashbackCategoryMccService _service;

        public CashbackCategoryMccController(IMapper mapper, ICashbackCategoryMccService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet("cashback/categories/{categoryId}/mcc")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<CashbackCategoryMcc>>> GetList(int categoryId)
        {
            var userId = User.GetUserId();

            var models = await _service.GetListWithAccessCheckAsync(userId, categoryId);

            return _mapper.Map<CashbackCategoryMcc[]>(models);
        }

        [HttpPost("cashback/categories/{categoryId}/mcc")]
        public async Task<ActionResult<CashbackCategoryMcc>> Add(CashbackCategoryMcc cashbackCategoryMcc)
        {
            var userId = User.GetUserId();

            var newModel = await _service.Add(userId, cashbackCategoryMcc);

            return CreatedAtAction("GetList", new { newModel.CategoryId }, newModel);
        }

        //     [HttpPut("cashback/categories/{categoryId}/mcc/{id}")]
        //     public async Task<ActionResult<Dto.CashbackCategoryMcc>> Update(int id, Dto.CashbackCategoryMcc product)
        //     {
        //         var userId = User.GetUserId();

        //var model = _mapper.Map<CashbackCategoryMcc>(product);

        //         var newModel = await _service.Edit(userId, model);

        //         return _mapper.Map<Dto.CashbackCategoryMcc>(newModel);
        //     }

        //     [HttpDelete("cashback/categories/{categoryId}/mcc/{id}")]
        //     public async Task<ActionResult> Delete(int id)
        //     {
        //         var userId = User.GetUserId();

        //         await _service.Delete(userId, id);

        //         return NoContent();
        //     }
    }
}
