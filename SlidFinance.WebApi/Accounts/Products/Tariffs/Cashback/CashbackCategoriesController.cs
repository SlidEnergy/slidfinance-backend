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
	[Route("api/v1/[controller]")]
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

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<CashbackCategory>>> GetList(int tariffId)
        {
            var userId = User.GetUserId();

            var products = await _service.GetListWithAccessCheckAsync(userId, tariffId);

            return products;
        }

        [HttpPost]
        public async Task<ActionResult<CashbackCategory>> Add(CashbackCategory category)
        {
            var userId = User.GetUserId();

            var newModel = await _service.Add(userId, category);

            return CreatedAtAction("GetList", new { newModel.TariffId }, newModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CashbackCategory>> Update(int id, CashbackCategory category)
        {
            var userId = User.GetUserId();

            var editModel = await _service.Edit(userId, category);

            return editModel;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.Delete(userId, id);

            return NoContent();
        }
    }
}
