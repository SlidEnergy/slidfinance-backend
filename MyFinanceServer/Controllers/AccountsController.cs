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
using AutoMapper;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public sealed class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountDataSaver _accountDataSaver;
        private readonly IMapper _mapper;

        public AccountsController(ApplicationDbContext context, IAccountDataSaver accountDataSaver, IMapper mapper)
        {
            _context = context;
            _accountDataSaver = accountDataSaver;
            _mapper = mapper;
        }

        // GET: api/Transactions
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Account>>> GetAccounts()
        {
            var userId = Int32.Parse(User.GetUserId());
            return await _context.Accounts.Where(x=>x.Bank.User.Id == userId).Select(x => _mapper.Map<Dto.Account>(x)).ToListAsync();
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> PatchAccountData(int id, PatchAccountDataBindingModel accountData)
        {
            // TODO: добавить подробное протоколирование, т.к. метод содержит логику

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
                Description = x.Description,
                BankCategory = x.Category ?? "",
                Mcc = x.Mcc
            }).ToList());

            return  NoContent();
        }
    }
}
