using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Models;

namespace MyFinanceServer.Controllers
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
            //var userId = Int32.Parse(User.FindFirst(JwtRegisteredClaimNames.NameId).Value);
            return await _context.Banks.ToListAsync();//.Include(x=>x.User).Where(x=>x.Id == userId).ToListAsync();
        }
    }
}
