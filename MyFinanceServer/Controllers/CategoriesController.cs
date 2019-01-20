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
using MyFinanceServer.Models;

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
        public async Task<ActionResult<IEnumerable<Dto.Category>>> GetCategory()
        {
            var userId = Int32.Parse(User.GetUserId());
            return await _context.Category.Where(x=>x.User.Id == userId).Select(x=> _mapper.Map<Dto.Category>(x)).ToListAsync();
        }
    }
}
