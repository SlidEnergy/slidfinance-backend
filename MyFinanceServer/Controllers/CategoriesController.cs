using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;


namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            var user = await _context.Users
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Unauthorized();

            var newCategory = user.AddCategory(category.Title);

            await _context.SaveChangesAsync();

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

            var category = await _context.Categories.Include(x => x.User).FirstOrDefaultAsync(b => b.Id == id && b.User.Id == userId);

            if (category == null)
                return NotFound();

            category.User.DeleteCategory(id);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
