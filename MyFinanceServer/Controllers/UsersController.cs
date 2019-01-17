using Microsoft.AspNetCore.Mvc;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users/current
        [HttpGet("current")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userId = Int32.Parse(User.GetUserId());

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            // обнуляем пароль, чтобы не выдавать.
            user.Password = null;
            return user;
        }
    }
}
