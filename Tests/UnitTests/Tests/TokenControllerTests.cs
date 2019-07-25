using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SlidFinance.WebApi;
using SlidFinance.App;
using NUnit.Framework;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
	public class TokenControllerTests : TestsBase
    {
        Mock<UserManager<ApplicationUser>> _manager;
		TokenController _controller;
		TokenGenerator _tokenGenerator;

		[SetUp]
        public void Setup()
        {
			var authSettings = SettingsFactory.CreateAuth();
			_tokenGenerator = new TokenGenerator(authSettings);
            var store = new Mock<IUserStore<ApplicationUser>>();

            _manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			var tokenService = new TokenService(_mockedDal.RefreshTokens, _tokenGenerator, authSettings);

			_controller = new TokenController(tokenService);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task RefreshToken_ShouldReturnTokens()
        {
			var token = _tokenGenerator.GenerateAccessToken(_user);
			var refreshToken = _tokenGenerator.GenerateRefreshToken();

			_refreshTokens.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(new RefreshToken("any", refreshToken, _user));
			
			var result = await _controller.Refresh(token, refreshToken);

            Assert.IsInstanceOf<ActionResult<Dto.TokenInfo>>(result);

            Assert.NotNull(result.Value.Token);
            Assert.NotNull(result.Value.RefreshToken);
			Assert.IsNotEmpty(result.Value.Token);
			Assert.IsNotEmpty(result.Value.RefreshToken);
			Assert.AreNotEqual(refreshToken, result.Value.Token);
			Assert.IsNotEmpty(refreshToken, result.Value.RefreshToken);
		}
	}
}