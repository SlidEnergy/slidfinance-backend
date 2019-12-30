using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class ImportControllerTests : TestsBase
    {
		ImportController _controller;
		Mock<ITokenService> _tokenService;
		Mock<IUsersService> _usersService;
		Mock<IApiImportService> _importService;

		[SetUp]
        public void Setup()
        {
			_tokenService = new Mock<ITokenService>();
			_usersService = new Mock<IUsersService>();
			_importService = new Mock<IApiImportService>();

			_controller = new ImportController(_autoMapper.Create(_db), _importService.Object, _tokenService.Object, _usersService.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task ImportToken_ShouldCallMethod()
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

		[Test]
		public async Task Import_ShouldCallMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Dto.ImportTransaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
				Mcc = 111
			};

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<PatchAccountDataBindingModel>())).ReturnsAsync(1);

			var result = await _controller.Import(new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.ImportTransaction[] { transaction1 } });

			_importService.Verify(x => x.Import(It.Is<string>(u => u == _user.Id), It.IsAny<PatchAccountDataBindingModel>()));
		}


		[Test]
		public async Task ImportWithoutMcc_ShouldCallMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Dto.ImportTransaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
			};

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<PatchAccountDataBindingModel>())).ReturnsAsync(1);

			var result = await _controller.Import(new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.ImportTransaction[] { transaction1 } });

			_importService.Verify(x => x.Import(It.Is<string>(u => u == _user.Id), It.IsAny<PatchAccountDataBindingModel>()));
		}
	}
}
