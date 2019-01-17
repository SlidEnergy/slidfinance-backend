using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Banks
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Bank>>> GetBanks()
        {
            var userId = Int32.Parse(User.GetUserId());
            return await _context.Banks.Where(x => x.User.Id == userId).ToListAsync();
        }
    }
}
