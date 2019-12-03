using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class UsersServiceTests: TestsBase
    {
		UsersService _service;
		Mock<UserManager<ApplicationUser>> _manager;
		AuthService _authService;
		Mock<ITokenService> _tokenService;

		[SetUp]
        public void Setup()
        {
			var authSettings = SettingsFactory.CreateAuth();
			var tokenGenerator = new TokenGenerator(authSettings);
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			_tokenService = new Mock<ITokenService>();
			_service = new UsersService(_manager.Object, _mockedDal);
			_authService = new AuthService(_manager.Object, _tokenService.Object);
		}

        [Test]
        public async Task Register_ShouldBeCallAddMethodWithRightArguments()
        {
            _manager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());

            var password = "Password1#";

            var result = await _service.CreateAccount(_user, password);

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

            var result = await _authService.CheckCredentialsAndGetToken(_user.Email, password);

            _manager.Verify(x => x.CheckPasswordAsync(
              It.Is<ApplicationUser>(u => u.UserName == _user.UserName && u.Email == _user.Email),
              It.Is<string>(p => p == password)), Times.Exactly(1));
        }
    }
}