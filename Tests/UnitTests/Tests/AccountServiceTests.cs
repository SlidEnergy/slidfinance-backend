using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class AccountServiceTests : TestsBase
    {
        private AccountsService _service;

        [SetUp]
        public void Setup()
        {
            _service = new AccountsService(_mockedDal, _db);
        }

        [Test]
        public async Task PatchAccount_ShouldSetBalance()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Code ="code_1", Bank = bank });

            _accounts.Setup(x => x.Update(It.IsAny<BankAccount>())).ReturnsAsync(account);
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(bank);

            account.Balance = 500;

            var patchedAccount = await _service.PatchAccount(_user.Id, account);

            Assert.AreEqual(500, patchedAccount.Balance);
        }

        [Test]
        public async Task PatchAccount_ShouldSetBank()
        {
            var bank1 = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var bank2 = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Bank = bank1 });
            
            _accounts.Setup(x => x.Update(It.IsAny<BankAccount>())).ReturnsAsync(account);
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(bank2);

            account.Bank = bank2;

            var patchedAccount = await _service.PatchAccount(_user.Id, account);

            Assert.AreEqual(bank2.Id, patchedAccount.Bank.Id);
        }

        [Test]
        public async Task PatchAccount_ShouldCallUpdateMethod()
        {
            var bank = await _dal.Banks.Add(new Bank() { Title = "Bank #1", User = _user });
            var account = await _dal.Accounts.Add(new BankAccount() { Title = "Account #1", Transactions = new List<Transaction>(), Bank = bank, Code = "Code #1"});

            _accounts.Setup(x => x.Update(It.IsAny<BankAccount>())).ReturnsAsync(account);
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(bank);

            var patchedAccount = await _service.PatchAccount(_user.Id, account);

            _accounts.Verify(x => x.Update(It.Is<BankAccount>(a =>
                a.Title == account.Title &&
                a.Balance == account.Balance
            )), Times.Once);
        }
    }
}