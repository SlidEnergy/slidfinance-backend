using System;
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
    public class CategoryTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetCategories_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetCategories_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = user
            });
            dbContext.Categories.Add(new Category()
            {
                Title = "Category #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var controller = new CategoriesController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}