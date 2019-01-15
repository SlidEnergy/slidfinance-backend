using System;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountDataSaver _accountDataSaver;

        public AccountsController(ApplicationDbContext dbContext, IAccountDataSaver accountDataSaver)
        {
            _dbContext = dbContext;
            _accountDataSaver = accountDataSaver;
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> PatchAccountData(int id, PatchAccountDataBindingModel accountData)
        {
            Console.Write("accountId: {0}, balance: {1}", id, accountData.Balance);
            var account = await _dbContext.Accounts
              .Include(x => x.Transactions)
              .SingleOrDefaultAsync(x => x.Id == id);

            if (account == null)
                NotFound();

            await _accountDataSaver.Save(account, accountData.Balance, accountData.Transactions.Select(x => new Transaction() {
                Account = account,
                Amount = x.Amount,
                DateTime = x.DateTime,
                Category = Category.None,
                Description = x.Category + " : " + x.Description
            }).ToList());

            return  NoContent();
        }
    }
}
