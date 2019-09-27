using Moq;
using NUnit.Framework;
using SlidFinance.App;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class TokenControllerTests : TestsBase
    {
		TokenController _controller;
		Mock<ITokenService> _tokenService;

		[SetUp]
        public void Setup()
        {
			_tokenService = new Mock<ITokenService>();

			_controller = new TokenController(_tokenService.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task RefreshToken_ShouldCallMethod()
        {
			var token = Guid.NewGuid().ToString();
			var refreshToken = Guid.NewGuid().ToString();

			_tokenService.Setup(x => x.RefreshToken(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new TokensCortage() { Token = token, RefreshToken = refreshToken });
			
			var result = await _controller.Refresh(token, refreshToken);

			_tokenService.Verify(x => x.RefreshToken(It.Is<string>(t => t == token), It.Is<string>(t => t == refreshToken)));
		}
	}
}