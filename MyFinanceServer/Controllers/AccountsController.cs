using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Domain;
using System;
using System.Collections.Generic;
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
        private readonly IMapper _mapper;

        public AccountsController(ApplicationDbContext context, IAccountDataSaver accountDataSaver, IMapper mapper)
        {
            _context = context;
            _accountDataSaver = accountDataSaver;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.BankAccount>>> GetList(int? bankId = null)
        {
            var userId = User.GetUserId();

            var accounts = await _context.Accounts
                .Where(x => (bankId == null || x.Bank.Id == bankId) && x.Bank.User.Id == userId)
                .OrderBy(x => x.Title)
                .ToListAsync();

            return _mapper.Map<Dto.BankAccount[]>(accounts);
        }

        [HttpPatch("{code}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> PatchAccountData(string code, PatchAccountDataBindingModel accountData)
        {
            // TODO: добавить подробное протоколирование, т.к. метод содержит логику

            Console.Write("accountId: {0}, balance: {1}, transactionsCount: {2}", code, accountData.Balance, accountData.Transactions.Count);

            var userId = User.GetUserId();

            Console.Write("userId: {0}", userId);

            var account = await _context.Accounts.Include(x => x.Transactions)
              .SingleOrDefaultAsync(x => x.Bank.User.Id == userId && x.Code == code);

            if (account == null)
                return NotFound();

            Console.Write("Account found");

            await _accountDataSaver.Save(userId, account, accountData.Balance, accountData.Transactions.Select(x => new Transaction() {
                Account = account,
                Amount = x.Amount,
                DateTime = x.DateTime,
                Description = x.Description ?? "",
                BankCategory = x.Category ?? "",
                Mcc = x.Mcc
            }).ToList());

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Dto.BankAccount>> Add(AddBankAccountBindingModel account)
        {
            var userId = User.GetUserId();

            var bank = await _context.Banks
                .OrderBy(x => x.Title)
                .FirstOrDefaultAsync(x => x.Id == account.BankId && x.User.Id == userId);

            var newAccount = bank.LinkAccount(account.Title, account.Code, account.Balance, account.CreditLimit);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetList", _mapper.Map<Dto.BankAccount>(newAccount));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EditBankAccountBindingModel account)
        {
            var userId = User.GetUserId();

            var editAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.Bank.User.Id == userId);

            editAccount.Rename(account.Title);
            editAccount.ChangeCode(account.Code);
            editAccount.SetBalance(account.Balance);
            editAccount.ChangeCreditLimit(account.CreditLimit);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            var account = await _context.Accounts
                .Include(x => x.Bank)
                .FirstOrDefaultAsync(x => x.Id == id && x.Bank.User.Id == userId);

            if (account == null)
                return NotFound();

            account.Bank.UnlinkAccount(id);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
