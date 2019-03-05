using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using MyFinanceServer.Shared;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CategoriesService _categoriesService;

        public CategoriesController(ApplicationDbContext context, IMapper mapper, CategoriesService categoriesService)
        {
            _context = context;
            _mapper = mapper;
            _categoriesService = categoriesService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dto.Category>>> GetList()
        {
            var userId = User.GetUserId();

            return await _context.Categories
                .Where(x => x.User.Id == userId)
                .OrderBy(x => x.Order)
                .Select(x => _mapper.Map<Dto.Category>(x))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Category>> Add(Dto.Category category)
        {
            var userId = User.GetUserId();

            var newCategory = await _categoriesService.AddCategory(userId, category.Title);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Category>(newCategory));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Dto.Category category)
        {
            var userId = User.GetUserId();

            var user = await _context.Users
               .Include(x => x.Categories)
               .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Unauthorized();

            var editCategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == userId);

            editCategory.Rename(category.Title);

            user.ReorderCategories(editCategory, category.Order);

            await _context.SaveChangesAsync();

            return NoContent();
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
