using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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

		[SetUp]
		public void Setup()
		{
			var authSettings = SettingsFactory.CreateAuth();
			var claimsGenerator = new ClaimsGenerator(Options.Create(new IdentityOptions()));
			var tokenGenerator = new TokenGenerator(authSettings, claimsGenerator);
			var store = new Mock<IUserStore<ApplicationUser>>();

			_manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
			_service = new UsersService(_manager.Object, _mockedDal);
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
	}
}