﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;
using MyFinanceServer.Domain;
using MyFinanceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public sealed class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountDataSaver _accountDataSaver;

        public AccountController(ApplicationDbContext dbContext, IAccountDataSaver accountDataSaver)
        {
            _dbContext = dbContext;
            _accountDataSaver = accountDataSaver;
        }

        // POST: api/account/id
        [HttpPatch]
        public async Task<ActionResult> PatchAccountData(int id, PatchAccountDataBindingModel accountData)
        {
            var account = await _dbContext.Accounts
              .Include(x => x.Transactions)
              .SingleOrDefaultAsync(x => x.Id == id);

            if (account == null)
                NotFound();

            await _accountDataSaver.Save(account, accountData.Balance, accountData.Transactions.Select(x => new Transaction() {
                Amount = x.Amount,
                DateTime = x.DateTime,
                Category = Category.None,
                Description = x.Category + " : " + x.Description
            }).ToList());

            return  NoContent();
        }
    }
}
