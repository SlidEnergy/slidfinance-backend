using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class MccServiceTests : TestsBase
    {
        MccService _service;

        [SetUp]
        public void Setup()
        {
            _service = new MccService(_mockedDal);
        }

		[Test]
		public async Task GetMccList_ShouldBeCallGetListMethod()
		{
			_mcc.Setup(x => x.GetList()).ReturnsAsync(new List<Mcc>());

			var result = await _service.GetListAsync();

			_mcc.Verify(x => x.GetList(), Times.Exactly(1));
		}
	}
}