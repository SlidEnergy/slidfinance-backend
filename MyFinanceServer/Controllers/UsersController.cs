using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System;
using System.Threading.Tasks;
using AutoMapper;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users/current
        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dto.User>> GetCurrentUser()
        {
            var userId = Int32.Parse(User.GetUserId());

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<Dto.User>(user);
        }
    }
}
