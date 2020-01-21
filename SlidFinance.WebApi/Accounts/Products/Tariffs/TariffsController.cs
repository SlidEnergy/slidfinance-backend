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
    public sealed class TariffsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductTariffsService _service;

        public TariffsController(IMapper mapper, IProductTariffsService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet("products/{productId}/tariffs")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.ProductTariff>>> GetList(int productId)
        {
            var userId = User.GetUserId();

            var products = await _service.GetListWithAccessCheckAsync(userId, productId);

            return _mapper.Map<Dto.ProductTariff[]>(products);
        }

        [HttpPatch("products/{productId}/tariffs/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dto.ProductTariff>> Patch(int id, JsonPatchDocument<Dto.ProductTariff> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

			var model = await _service.GetByIdWithAccessCheck(userId, id);

			var dto = _mapper.Map<Dto.ProductTariff>(model);
			patchDoc.ApplyTo(dto);

            _mapper.Map(dto, model);

            var patchetProduct = await _service.Edit(userId, model);

            return _mapper.Map<Dto.ProductTariff>(patchetProduct);
        }

        [HttpPost("products/{productId}/tariffs")]
        public async Task<ActionResult<Dto.ProductTariff>> Add(Dto.ProductTariff tariff)
        {
            var userId = User.GetUserId();

			var model = new ProductTariff();

			_mapper.Map(tariff, model);

			var newTariff = await _service.Add(userId, model);

            return CreatedAtAction("GetList", new { newTariff.ProductId }, _mapper.Map<Dto.ProductTariff>(newTariff));
        }

        [HttpPut("products/{productId}/tariffs/{id}")]
        public async Task<ActionResult<Dto.ProductTariff>> Update(int id, Dto.ProductTariff tariff)
        {
            var userId = User.GetUserId();

			var model = _mapper.Map<ProductTariff>(tariff);

            var editAccount = await _service.Edit(userId, model);

            return _mapper.Map<Dto.ProductTariff>(editAccount);
        }

        [HttpDelete("products/{productId}/tariffs/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.Delete(userId, id);

            return NoContent();
        }
    }
}
