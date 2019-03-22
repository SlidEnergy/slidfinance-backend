using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Core;
using MyFinanceServer.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CategoriesService _categoriesService;

        public CategoriesController( IMapper mapper, CategoriesService categoriesService)
        {
            _mapper = mapper;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dto.Category>>> GetList()
        {
            var userId = User.GetUserId();

            var categories = await _categoriesService.GetList(userId);
            return _mapper.Map<Dto.Category[]>(categories);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Category>> Add(Dto.Category category)
        {
            var userId = User.GetUserId();

            var newCategory = await _categoriesService.AddCategory(userId, category.Title);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Category>(newCategory));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.Category>> Update(int id, Dto.Category category)
        {
            var userId = User.GetUserId();

            var editedCategory = await _categoriesService.EditCategory(userId, id, category.Title, category.Order);

            return _mapper.Map<Dto.Category>(editedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _categoriesService.DeleteCategory(userId, id);

            return NoContent();
        }
    }
}
