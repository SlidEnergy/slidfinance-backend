using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Domain;
using MyFinanceServer.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public sealed class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountDataSaver _accountDataSaver;

        public AccountsController(ApplicationDbContext context, IAccountDataSaver accountDataSaver)
        {
            _context = context;
            _accountDataSaver = accountDataSaver;
        }

        // GET: api/Transactions
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var userId = Int32.Parse(User.GetUserId());
            return await _context.Accounts.Where(x=>x.Bank.User.Id == userId).ToListAsync();
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> PatchAccountData(int id, PatchAccountDataBindingModel accountData)
        {
            Console.Write("accountId: {0}, balance: {1}, transactionsCount: {2}", id, accountData.Balance, accountData.Transactions.Count);

            var userId = Int32.Parse(User.GetUserId());

            Console.Write("userId: {0}", userId);

            var account = await _context.Accounts.Include(x => x.Transactions)
              .SingleOrDefaultAsync(x => x.Bank.User.Id == userId && x.Id == id);

            if (account == null)
                NotFound();

            Console.Write("Account found");

            await _accountDataSaver.Save(account, accountData.Balance, accountData.Transactions.Select(x => new Transaction() {
                Account = account,
                Amount = x.Amount,
                DateTime = x.DateTime,
                Description = x.Category + " : " + x.Description
            }).ToList());

            return  NoContent();
        }
    }
}
