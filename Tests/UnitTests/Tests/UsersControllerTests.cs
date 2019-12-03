using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class UsersControllerTests : TestsBase
    {
		private UsersController _controller;
		private Mock<IUsersService> _service;
		private Mock<IAuthService> _authService;

	   [SetUp]
        public void Setup()
        {
			_service = new Mock<IUsersService>();
			_authService = new Mock<IAuthService>();

			_controller = new UsersController(_autoMapper.Create(_db), _service.Object, _authService.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetCurrentUser_ShouldReturnUser()
        {
            _service.Setup(x => x.GetById(It.IsAny<string>())).Returns(Task.FromResult(_user));

            var result = await _controller.GetCurrentUser();

            Assert.IsInstanceOf<ActionResult<Dto.User>>(result);

			_service.Verify(x => x.GetById(It.Is<string>(u => u == _user.Id)));
        }

        [Test]
        public async Task Login_ShouldReturnTokenAndEmail()
        {
			_authService.Setup(x => x.CheckCredentialsAndGetToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokensCortage());

            var userBindingModel = new LoginBindingModel() { Email = _user.Email, Password = "Password #1" };

            var result = await _controller.GetToken(userBindingModel);

			_authService.Verify(x => x.CheckCredentialsAndGetToken(It.Is<string>(e => e == userBindingModel.Email), It.Is<string>(p => p == userBindingModel.Password)));
        }

		[Test]
		public async Task Register_ShouldReturnUser()
		{
			var password = "Password #2";
			var email = "test2@email.com";

			_service.Setup(x => x.CreateAccount(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

			var registerBindingModel = new RegisterBindingModel() { Email = email, Password = password, ConfirmPassword = password };

			var result = await _controller.Register(registerBindingModel);

			Assert.NotNull(((CreatedResult)result.Result).Value);

			_service.Verify(x => x.CreateAccount(It.Is<ApplicationUser>(u => u.Email == registerBindingModel.Email && u.UserName == registerBindingModel.Email),
				It.Is<string>(p => p == registerBindingModel.Password)));
		}
	}
}