using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly IMapper _mapper;
        private readonly AccountsService _service;

        public AccountsController(IMapper mapper, AccountsService service)
        {
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

        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Dto.BankAccount>> PatchAccount(int id, JsonPatchDocument<Dto.BankAccount> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

            var account = await _service.GetById(userId, id);

            var dto = _mapper.Map<Dto.BankAccount>(account);
            patchDoc.ApplyTo(dto);

            _mapper.Map(dto, account);

            var patchedAccount = await _service.PatchAccount(userId, account);

            return _mapper.Map<Dto.BankAccount>(patchedAccount);
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
