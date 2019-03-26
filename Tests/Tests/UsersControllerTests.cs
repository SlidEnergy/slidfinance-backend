using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class UsersControllerTests : TestsBase
    {
        UsersService _service;
        Mock<UserManager<ApplicationUser>> _manager;

        [SetUp]
        public void Setup()
        {
            var tokenGenerator = new TokenGenerator(Options.Create(new AppSettings() { Secret = "Very very very long secret #1" }));
            var store = new Mock<IUserStore<ApplicationUser>>();

            _manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _service = new UsersService(_manager.Object, tokenGenerator);
        }

        [Test]
        public async Task GetCurrentUser_Ok()
        {
            _manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(_user));

            var controller = new UsersController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetCurrentUser();

            Assert.IsInstanceOf<ActionResult<Api.Dto.User>>(result);

            Assert.AreEqual(_user.Id, result.Value.Id);
            Assert.AreEqual(_user.Email, result.Value.Email);
        }

        [Test]
        public async Task GetToken_Ok()
        {
            _manager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(_user));

            var userBindingModel = new LoginBindingModel() { Email = _user.Email, Password = "Password #1" };

            var controller = new UsersController(_autoMapper.Create(_db), _service);
            var result = await controller.Login(userBindingModel);

            Assert.NotNull(result.Value.Token);
            Assert.IsNotEmpty(result.Value.Token);
            Assert.AreEqual(_user.Email, result.Value.Email);
        }
    }
}