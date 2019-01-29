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
            return await _context.Categories.Where(x=>x.User.Id == userId).Select(x=> _mapper.Map<Dto.Category>(x)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Category>> AddCategory(Dto.Category category)
        {
            var userId = User.GetUserId();
            var user = await _context.Users.FindAsync(userId);

            var newCategory = new Category { Title = category.Title, User = user};
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetList", new { id = newCategory.Id }, _mapper.Map<Dto.Category>(newCategory));
        }

        // DELETE: api/Rules/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Category>> DeleteCategory(string id)
        {
            var userId = User.GetUserId();

            var category = await _context.Categories.SingleOrDefaultAsync(x => x.User.Id == userId && x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<Dto.Category>(category);
        }
    }
}
