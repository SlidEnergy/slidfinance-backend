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
		Mock<IMccService> _mccService;
		Mock<IImportService> _importService;

		[SetUp]
        public void Setup()
        {
			_tokenService = new Mock<ITokenService>();
			_usersService = new Mock<IUsersService>();
			_importService = new Mock<IImportService>();
			_mccService = new Mock<IMccService>();

			_controller = new ImportController(_autoMapper.Create(_db), _importService.Object, _tokenService.Object, _usersService.Object, _mccService.Object);
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
		public async Task ImportWithNewMcc_ShouldCallAddMccMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Dto.Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				Mcc = 111
			};

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Mcc>());
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync((Mcc x) => x);

			var result = await _controller.Import(new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.Transaction[] { transaction1 } });

			_mccService.Verify(x => x.AddAsync(It.Is<Mcc>(m => m.Code == transaction1.Mcc.Value.ToString("D4") && m.IsSystem == false)));
		}

		[Test]
		public async Task ImportWithExistMcc_ShouldNotCallAddMccMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var mcc = new Mcc() { Code = "0111" };
			_db.Mcc.Add(mcc);
			var transaction1 = new Dto.Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				Mcc = 111
			};
			await _db.SaveChangesAsync();

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Mcc>() { mcc });
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync((Mcc x) => x);

			var result = await _controller.Import(new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.Transaction[] { transaction1 } });

			_mccService.Verify(x => x.AddAsync(It.IsAny<Mcc>()), Times.Never);
		}
	}
}
