using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Route("api/v1/[controller]")]
    [ApiController]
    public sealed class AccountsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountsService _service;

        public AccountsController(IMapper mapper, IAccountsService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [Authorize(Policy = Policy.MustBeAllOrImportAccessMode)]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.BankAccount>>> GetList(int? bankId = null)
        {
            var userId = User.GetUserId();

            var accounts = await _service.GetListWithAccessCheckAsync(userId, bankId);

            return _mapper.Map<Dto.BankAccount[]>(accounts);
        }

        [Authorize(Policy = Policy.MustBeAllAccessMode)]
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dto.BankAccount>> PatchAccount(int id, JsonPatchDocument<Dto.BankAccount> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

            var account = await _service.GetByIdWithAccessCheckAsync(userId, id);

            var dto = _mapper.Map<Dto.BankAccount>(account);
            patchDoc.ApplyTo(dto);

            _mapper.Map(dto, account);

            var patchedAccount = await _service.PatchAccount(userId, account);

            return _mapper.Map<Dto.BankAccount>(patchedAccount);
        }

        [Authorize(Policy = Policy.MustBeAllAccessMode)]
        [HttpPost]
        public async Task<ActionResult<Dto.BankAccount>> Add(Dto.BankAccount account)
        {
            var userId = User.GetUserId();

            var newAccount = await _service.AddAccount(userId, _mapper.Map<BankAccount>(account));

            return CreatedAtAction("GetList", _mapper.Map<Dto.BankAccount>(newAccount));
        }

        [Authorize(Policy = Policy.MustBeAllAccessMode)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Dto.BankAccount>> Update(int id, Dto.BankAccount account)
        {
            var userId = User.GetUserId();

            var model = _mapper.Map<BankAccount>(account);

            var editAccount = await _service.Update(userId, model);

            return _mapper.Map<Dto.BankAccount>(editAccount);
        }

        [Authorize(Policy = Policy.MustBeAllAccessMode)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.DeleteAccount(userId, id);

            return NoContent();
        }
    }
}
