using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class MccServiceTests : TestsBase
    {
        MccService _service;

        [SetUp]
        public void Setup()
        {
            _service = new MccService(_db);
        }

		[Test]
		public async Task GetMccList_ShouldBeCallGetListMethod()
		{
			await _db.CreateMcc("4814");
			await _db.CreateMcc("4812");

			var result = await _service.GetListAsync();

			Assert.AreEqual(2, result.Count());
		}
	}
}