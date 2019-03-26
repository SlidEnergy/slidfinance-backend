using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using MyFinanceServer.Shared;
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
        private readonly AccountsService _service;

        public AccountsController(ApplicationDbContext context, IAccountDataSaver accountDataSaver, IMapper mapper, AccountsService service)
        {
            _context = context;
            _accountDataSaver = accountDataSaver;
            _mapper = mapper;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.BankAccount>>> GetList(int? bankId = null)
        {
            var userId = User.GetUserId();

            var accounts = await _service.GetList(userId, bankId);

            return _mapper.Map<Dto.BankAccount[]>(accounts);
        }

        [HttpPatch("{code}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Dto.BankAccount>> PatchAccountData(string code, PatchAccountDataBindingModel accountData)
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

            return _mapper.Map<Dto.BankAccount>(account);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.BankAccount>> Add(AddBankAccountBindingModel account)
        {
            var userId = User.GetUserId();

            var newAccount = await _service.AddAccount(userId, account.BankId, account.Title, account.Code, account.Balance, account.CreditLimit);

            return CreatedAtAction("GetList", _mapper.Map<Dto.BankAccount>(newAccount));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.BankAccount>> Update(int id, EditBankAccountBindingModel account)
        {
            var userId = User.GetUserId();

            var editAccount = await _service.EditAccount(userId, id, account.Title, account.Code, account.Balance, account.CreditLimit);

            return _mapper.Map<Dto.BankAccount>(editAccount);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.DeleteAccount(userId, id);

            return NoContent();
        }
    }
}
