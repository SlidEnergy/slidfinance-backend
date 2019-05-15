using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class AccountControllerTests : TestsBase
    {
        private AccountsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new AccountsService(_mockedDal);
        }

        [Test]
        public async Task GetAccounts_ShouldReturnList()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            await _dal.Accounts.Add(new BankAccount() { Title = "Account #2", Bank = bank });

            _accounts.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(bank.Accounts.ToList());

            var controller = new AccountsController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}