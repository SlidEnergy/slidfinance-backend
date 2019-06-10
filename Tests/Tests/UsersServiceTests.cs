using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using MyFinanceServer.Core;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
	public class UsersServiceTests: TestsBase
    {
		UsersService _service;
		Mock<UserManager<ApplicationUser>> _manager;

		[SetUp]
        public void Setup()
        {
			var options = Options.Create(new AppSettings() { Secret = "Very very very long secret #1" });
			var tokenGenerator = new TokenGenerator(options);
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			var tokenService = new TokenService(_mockedDal.RefreshTokens, tokenGenerator, options);
			_service = new UsersService(_manager.Object, tokenGenerator, tokenService);
		}

        [Test]
        public async Task Register_ShouldBeCallAddMethodWithRightArguments()
        {
            _manager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var password = "Password1#";

            var result = await _service.Register(_user, password);

            _manager.Verify(x => x.CreateAsync(
                It.Is<ApplicationUser>(u=> u.UserName == _user.UserName && u.Email == _user.Email), 
                It.Is<string>(p=>p == password)), Times.Exactly(1));
        }

        [Test]
        public async Task Login_ShouldBeCallAddMethodWithRightArguments()
        {
            var password = "Password1#";

            _manager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(_user));

            var result = await _service.Login(_user.Email, password);

            _manager.Verify(x => x.CheckPasswordAsync(
              It.Is<ApplicationUser>(u => u.UserName == _user.UserName && u.Email == _user.Email),
              It.Is<string>(p => p == password)), Times.Exactly(1));
        }
    }
}