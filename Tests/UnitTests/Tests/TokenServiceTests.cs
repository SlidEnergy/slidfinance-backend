using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SlidFinance.WebApi;
using SlidFinance.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Security.Claims;

namespace SlidFinance.WebApi.UnitTests
{
    public class TokenServiceTests : TestsBase
	{
		private TokenService _service;
		Mock<ITokenGenerator> _tokenGenerator;

		[SetUp]
        public void Setup()
        {
			var authSettings = SettingsFactory.CreateAuth();
			_tokenGenerator = new Mock<ITokenGenerator>();
			
			_service = new TokenService(_mockedDal.RefreshTokens, _tokenGenerator.Object, authSettings);
        }

		[Test]
		[TestCase(AccessMode.All)]
		[TestCase(AccessMode.Import)]
		public async Task GenerateAccessAndRefreshTokens_ShouldBeCalledMethods(AccessMode accessMode)
		{
			var accessToken = Guid.NewGuid().ToString();
			var refreshToken = Guid.NewGuid().ToString();

			_tokenGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<AccessMode>())).Returns(accessToken);
			_tokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
			_refreshTokens.Setup(x => x.Add(It.IsAny<RefreshToken>())).ReturnsAsync(new RefreshToken("any", refreshToken, _user));

			await _service.GenerateAccessAndRefreshTokens(_user, accessMode);

			_tokenGenerator.Verify(x => x.GenerateAccessToken(It.Is<ApplicationUser>(user => user.Id == _user.Id), It.Is<AccessMode>(mode => mode == accessMode)));
			_tokenGenerator.Verify(x => x.GenerateRefreshToken());
			_refreshTokens.Verify(x => x.Add(It.Is<RefreshToken>(token => token.Token == refreshToken && token.User.Id == _user.Id)));
		}


		[Test]
		[TestCase(AccessMode.All)]
		[TestCase(AccessMode.Import)]
		public async Task RefreshToken_ShouldCalledMethods(AccessMode accessMode)
		{
			var authSettings = SettingsFactory.CreateAuth();
			var tokenGenerator = new TokenGenerator(authSettings);

			var token = tokenGenerator.GenerateAccessToken(_user, AccessMode.All);
			var refreshToken = tokenGenerator.GenerateRefreshToken();

			var newAccessToken = Guid.NewGuid().ToString();
			var newRefreshToken = Guid.NewGuid().ToString();

			_tokenGenerator.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(newAccessToken);
			_tokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns(newRefreshToken);
			_refreshTokens.Setup(x => x.Find(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RefreshToken("any", refreshToken, _user));

			await _service.RefreshToken(token, refreshToken);

			_tokenGenerator.Verify(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()));
			_tokenGenerator.Verify(x => x.GenerateRefreshToken());
			_refreshTokens.Verify(x => x.Find(It.Is<string>(userId => userId == _user.Id), It.Is<string>(t => t == refreshToken)));
		}
	}
}