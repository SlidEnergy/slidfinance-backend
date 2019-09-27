using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class ImportControllerTests : TestsBase
    {
		ImportController _controller;
		Mock<ITokenService> _tokenService;
		Mock<IUsersService> _usersService;

		[SetUp]
        public void Setup()
        {
			_tokenService = new Mock<ITokenService>();
			_usersService = new Mock<IUsersService>();
			var importService = new Mock<IImportService>();

			_controller = new ImportController(_autoMapper.Create(_db), importService.Object, _tokenService.Object, _usersService.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task RefreshToken_ShouldCallMethod()
        {
			var token = Guid.NewGuid().ToString();
			var refreshToken = Guid.NewGuid().ToString();

			_usersService.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
			_tokenService.Setup(x => x.GenerateAccessAndRefreshTokens(It.IsAny<ApplicationUser>(), It.IsAny<AccessMode>()))
				.ReturnsAsync(new TokensCortage() { Token = token, RefreshToken = refreshToken });
			
			var result = await _controller.GetToken();

			_usersService.Verify(x => x.GetById(It.Is<string>(id => id == _user.Id)));
			_tokenService.Verify(x => x.GenerateAccessAndRefreshTokens(
				It.Is<ApplicationUser>(u => u.Id == _user.Id), 
				It.Is<AccessMode>(mode => mode == AccessMode.Import)));
		}
	}
}