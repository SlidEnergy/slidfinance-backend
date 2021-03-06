﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    [Authorize(Policy = Policy.MustBeAllAccessMode)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        ITransactionsService _service;

        public TransactionsController(IMapper mapper, ITransactionsService service)
        {
            _mapper = mapper;
            _service = service;
        }

        // GET: api/Transactions
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Dto.Transaction>>> GetList(int? accountId, int? categoryId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
			if(startDate.HasValue)
				startDate = startDate.Value.ToUniversalTime();
			if(endDate.HasValue)
				endDate = endDate.Value.ToUniversalTime();

			var userId = User.GetUserId();

            var transactions = await _service.GetListWithAccessCheckAsync(userId, accountId, categoryId, startDate, endDate);

            return _mapper.Map<Dto.Transaction[]>(transactions);
        }

        // POST: api/account/id
        [HttpPatch("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dto.Transaction>> Patch(int id, JsonPatchDocument<Dto.Transaction> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var userId = User.GetUserId();

            var transaction = await _service.GetById(userId, id);

            var dto = _mapper.Map<Dto.Transaction>(transaction);
            patchDoc.ApplyTo(dto);

            _mapper.Map(dto, transaction);

            var patchedTransaction = await _service.PatchTransaction(userId, transaction);

            return _mapper.Map<Dto.Transaction>(patchedTransaction);
        }

        [HttpPost]
        public async Task<ActionResult<Dto.Transaction>> Add(Dto.Transaction transaction)
        {
            var userId = User.GetUserId();

            var t = _mapper.Map<Transaction>(transaction);

            var newTransaction = await _service.AddTransaction(userId, t);

            return CreatedAtAction("GetList", _mapper.Map<Dto.Transaction>(newTransaction));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Dto.Transaction>> Delete(int id)
        {
            var userId = User.GetUserId();

            await _service.DeleteTransaction(userId, id);

            return NoContent();
        }
    }
}
