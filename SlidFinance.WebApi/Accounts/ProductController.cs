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
    public sealed class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductsService _service;

        public ProductController(IMapper mapper, IProductsService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Product>>> GetList()
        {
            var userId = User.GetUserId();

            var products = await _service.GetListWithAccessCheckAsync(userId);

            return _mapper.Map<Dto.Product[]>(products);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dto.Product>> PatchProduct(int id, JsonPatchDocument<Dto.Product> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

			var model = await _service.GetByIdWithAccessCheck(userId, id);

			var dto = _mapper.Map<Dto.Product>(model);
			patchDoc.ApplyTo(dto);

            _mapper.Map(dto, model);

            var patchetProduct = await _service.EditProduct(userId, model);

            return _mapper.Map<Dto.Product>(patchetProduct);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Product>> Add(Dto.Product product)
        {
            var userId = User.GetUserId();

			var model = new Product();

			_mapper.Map(product, model);

			var newProduct = await _service.AddProduct(userId, model);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Product>(newProduct));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.Product>> Update(int id, Dto.Product product)
        {
            var userId = User.GetUserId();

			var model = await _service.GetByIdWithAccessCheck(userId, id);

			_mapper.Map(product, model);

            var editAccount = await _service.EditProduct(userId, model);

            return _mapper.Map<Dto.Product>(editAccount);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.DeleteProduct(userId, id);

            return NoContent();
        }
    }
}
