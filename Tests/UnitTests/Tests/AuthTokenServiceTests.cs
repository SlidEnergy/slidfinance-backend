using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class AuthTokenServiceTests : TestsBase
	{
		private AuthTokenService _service;
		Mock<ITokenGenerator> _tokenGenerator;
		Mock<UserManager<ApplicationUser>> _manager;

		[SetUp]
        public void Setup()
        {
			var authSettings = SettingsFactory.CreateAuth();
			_tokenGenerator = new Mock<ITokenGenerator>();
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			var authTokenService = new Mock<IAuthTokenService>();
			_service = new AuthTokenService(_manager.Object, _mockedDal.AuthTokens);
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