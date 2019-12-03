using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class TokenServiceTests : TestsBase
	{
		private TokenService _service;
		Mock<ITokenGenerator> _tokenGenerator;
		Mock<UserManager<ApplicationUser>> _manager;

		[SetUp]
        public void Setup()
        {
			var authSettings = SettingsFactory.CreateAuth();
			_tokenGenerator = new Mock<ITokenGenerator>();
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			_service = new TokenService(_mockedDal.AuthTokens, _tokenGenerator.Object, authSettings, _manager.Object);
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
			_authTokens.Setup(x => x.Add(It.IsAny<AuthToken>())).ReturnsAsync(new AuthToken("any", refreshToken, _user, AuthTokenType.RefreshToken));

			await _service.GenerateAccessAndRefreshTokens(_user, accessMode);

			_tokenGenerator.Verify(x => x.GenerateAccessToken(It.Is<ApplicationUser>(user => user.Id == _user.Id), It.Is<AccessMode>(mode => mode == accessMode)));
			_tokenGenerator.Verify(x => x.GenerateRefreshToken());
			_authTokens.Verify(x => x.Add(It.Is<AuthToken>(token => token.Token == refreshToken && token.User.Id == _user.Id && token.Type == AuthTokenType.RefreshToken)));
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
			_authTokens.Setup(x => x.FindRefreshToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AuthToken("any", refreshToken, _user, AuthTokenType.RefreshToken));

			await _service.RefreshToken(token, refreshToken);

			_tokenGenerator.Verify(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()));
			_tokenGenerator.Verify(x => x.GenerateRefreshToken());
			_authTokens.Verify(x => x.FindRefreshToken(It.Is<string>(userId => userId == _user.Id), It.Is<string>(t => t == refreshToken)));
		}

		[Test]
		[TestCase(AuthTokenType.RefreshToken)]
		[TestCase(AuthTokenType.TelegramChatId)]
		public async Task AddToken_ShouldNotBeException(AuthTokenType type)
		{
			_manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_user);
			_authTokens.Setup(x => x.FindAnyToken(It.IsAny<string>())).ReturnsAsync(new AuthToken());
			_authTokens.Setup(x => x.Add(It.IsAny<AuthToken>())).ReturnsAsync(new AuthToken());

			var token = Guid.NewGuid().ToString();

			await _service.AddToken(_user.Id, token, type);

			_manager.Verify(x => x.FindByIdAsync(It.Is<string>(id => id == _user.Id)));
			_authTokens.Verify(x => x.FindAnyToken((It.Is<string>(t => t == token))));
			_authTokens.Verify(x => x.Add((It.Is<AuthToken>(t => t.User == _user && t.Token == token))));
		}
	}
}