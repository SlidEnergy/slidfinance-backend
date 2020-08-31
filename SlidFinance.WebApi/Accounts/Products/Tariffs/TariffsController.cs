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
	public sealed class TariffsController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IProductTariffsService _service;

		public TariffsController(IMapper mapper, IProductTariffsService service)
		{
			_mapper = mapper;
			_service = service;
		}

		[HttpGet]
		[ProducesResponseType(200)]
		public async Task<ActionResult<IEnumerable<ProductTariff>>> GetList(int productId)
		{
			var userId = User.GetUserId();

			var products = await _service.GetListWithAccessCheckAsync(userId, productId);

			return products;
		}

		[HttpPatch("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<ProductTariff>> Patch(int id, JsonPatchDocument<ProductTariff> patchDoc)
		{
			if (patchDoc == null)
				return BadRequest();

			var userId = User.GetUserId();

			var model = await _service.GetByIdWithAccessCheck(userId, id);

			patchDoc.ApplyTo(model);

			var patchetProduct = await _service.Edit(userId, model);

			return patchetProduct;
		}

		[HttpPost]
		public async Task<ActionResult<ProductTariff>> Add(ProductTariff tariff)
		{
			var userId = User.GetUserId();

			var newTariff = await _service.Add(userId, tariff);

			return CreatedAtAction("GetList", new { newTariff.ProductId }, newTariff);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<ProductTariff>> Update(int id, ProductTariff tariff)
		{
			var userId = User.GetUserId();

			var editAccount = await _service.Edit(userId, tariff);

			return editAccount;
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
