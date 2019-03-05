using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyFinanceServer.Core;
using Microsoft.Extensions.Options;

namespace MyFinanceServer.Tests
{
    public class UsersServiceTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Register_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Register_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { UserName = "Email #1", Email = "Email #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var tokenGenerator = new TokenGenerator(Options.Create(new AppSettings() { Secret = "Very very very long secret #1" }));
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var password = "Password1#";

            var service = new UsersService(manager.Object, tokenGenerator);
            var result = await service.Register(user, password);

            manager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u=> u.UserName == user.UserName && u.Email == user.Email), 
                It.Is<string>(p=>p == password)), Times.Exactly(1));
        }

        [Test]
        public async Task Login_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Login_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "email@domain.com" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var password = "Password1#";

            var tokenGenerator = new TokenGenerator(Options.Create(new AppSettings() { Secret = "Very very very long secret #1" }));
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            var service = new UsersService(manager.Object, tokenGenerator);
            var result = await service.Login(user.Email, password);

            manager.Verify(x => x.CheckPasswordAsync(
              It.Is<ApplicationUser>(u => u.UserName == user.UserName && u.Email == user.Email),
              It.Is<string>(p => p == password)), Times.Exactly(1));
        }
    }
}