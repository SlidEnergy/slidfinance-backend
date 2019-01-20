﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class BankTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetBanks_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Get_Banks");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = user
            });
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var controller = new BanksController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.GetBanks();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}