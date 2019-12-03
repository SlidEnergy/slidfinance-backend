using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class TelegramServiceTests : TestsBase
	{
		private TelegramService _service;
		private Mock<ITokenService> _tokenService;

		[SetUp]
        public void Setup()
        {
			var botSettings = SettingsFactory.CreateTelegramBot();
			_tokenService = new Mock<ITokenService>();

			_service = new TelegramService(_tokenService.Object, botSettings);

		}

		[Test]
		public async Task ValidateTelegramData_ShouldNotBeException()
		{
			_tokenService.Setup(x => x.AddToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AuthTokenType>())).Returns(Task.CompletedTask);

			var telegramUser = new TelegramUser()
			{
				Auth_date = 1575256110,
				First_name = "FirstName",
				Hash = "74035677c5d604d9498b2d20938b40e6c55c717cd15822aa2c2d3e1f056dba51",
				Id = 123456789,
				Last_name = "LastName",
				Username = "Username"
			};

			await _service.ConnectTelegramUser(_user.Id, telegramUser);

			_tokenService.Verify(x => x.AddToken(
				It.Is<string>(u => u == _user.Id), 
				It.Is<string>(t => t == telegramUser.Id.ToString()), 
				It.Is<AuthTokenType>(t=>t == AuthTokenType.TelegramChatId)));
		}
	}
}