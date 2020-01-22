using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	[TestFixture]
    public class MerchantServiceTests : TestsBase
    {
        private MerchantService _service;

        [SetUp]
        public void Setup()
        {
            _service = new MerchantService(_db);
        }

        [Test]
        public async Task AddMerchant_ShouldBeAdded()
        {
            var mcc = new Mcc() { Code = "0100" };
            _db.Mcc.Add(mcc);
            await _db.SaveChangesAsync();

            var merchant = new Merchant() { MccId = mcc.Id, Name = Guid.NewGuid().ToString() };
            await _service.AddAsync(merchant);

            var count = await _db.Merchants.CountAsync();
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task AddMerchant_ShouldNotBeAdded()
        {
            var mcc = await _db.CreateMcc("0100");
            var merchant = new Merchant() { MccId = mcc.Id, Name = Guid.NewGuid().ToString() };
            _db.Merchants.Add(merchant);
            await _db.SaveChangesAsync();

            await _service.AddAsync(merchant);

            var count = await _db.Merchants.CountAsync();
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task GetList_ShouldBeListReturned()
        {
            var mcc = await _db.CreateMcc("0100");
            var merchant1 = await _db.CreateMerchant(mcc.Id);
			var merchant2 = await _db.CreateMerchant(mcc.Id);
            merchant1.IsPublic = true;
            merchant2.IsPublic = true;
            await _db.SaveChangesAsync();

            var result = await _service.GetListWithAccessCheckAsync(_user.Id);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetEmptyMerchantList_ShouldBeEmptyListReturned()
        {
            var result = await _service.GetListWithAccessCheckAsync(_user.Id);

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task EditMerchant_IsPublicShouldBeUpdated()
        {
            var mcc = await _db.CreateMcc("0100");
            var merchant = await _db.CreateMerchant(mcc.Id);

            var newMerchant = new Merchant() { Id = merchant.Id, Name = merchant.Name, IsPublic = true };

            await _service.EditMerchant(_user.Id, newMerchant);

            var editedMerchant = await _db.Merchants.FirstOrDefaultAsync(x => x.Id == newMerchant.Id && x.IsPublic == false);

            Assert.NotNull(editedMerchant);
        }

        [Test]
        public async Task EditMerchant_ShouldBeUpdated()
        {
            var mcc = await _db.CreateMcc("0100");
            var merchant = await _db.CreateMerchant(mcc.Id);

            var newMerchant = new Merchant() { Id = merchant.Id, Name = merchant.Name, DisplayName = "Merchant #1" };

            await _service.EditMerchant(_user.Id, newMerchant);

            var editedMerchant = await _db.Merchants.FirstOrDefaultAsync(x => x.Id == newMerchant.Id && x.DisplayName == newMerchant.DisplayName);

            Assert.NotNull(editedMerchant);
        }
    }
}