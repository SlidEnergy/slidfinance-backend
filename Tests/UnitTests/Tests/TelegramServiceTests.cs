using Microsoft.AspNetCore.Identity;
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
		Mock<UserManager<ApplicationUser>> _manager;

		[SetUp]
        public void Setup()
        {
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			var botSettings = SettingsFactory.CreateTelegramBot();
			
			_service = new TelegramService(_manager.Object, botSettings);
        }

		[Test]
		public async Task ValidateTelegramData_ShouldBeTrue()
		{
			_manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(_user);
			_manager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new IdentityResult());

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

			_manager.Verify(x => x.FindByIdAsync(It.Is<string>(id => id == _user.Id)));
			_manager.Verify(x => x.UpdateAsync(It.Is<ApplicationUser>(user => user.Id == _user.Id)));
		}
	}
}