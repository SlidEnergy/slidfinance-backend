using Moq;
using SlidFinance.WebApi;
using SlidFinance.App;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
	[TestFixture]
    public class MccControllerTests : TestsBase
    {
		private MccController _controller;

        [SetUp]
        public void Setup()
        {
            var service = new MccService(_mockedDal);
			_controller = new MccController(_autoMapper.Create(_db), service);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetMccList_ShouldBeListReturned()
        {
            await _dal.Mcc.Add(new Mcc()
            {
				Code = "1111",
                Title = "Category #1",
				Description = "Description #1",
				Category = MccCategory.Airlines
            });
            await _dal.Mcc.Add(new Mcc()
            {
				Code = "2222",
				Title = "Category #2",
				Description = "Description #2",
				Category = MccCategory.Airlines
			});

            _mcc.Setup(x => x.GetList()).Returns(_dal.Mcc.GetList());

            var result = await _controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task GetEmptyMccList_ShouldBeEmptyListReturned()
        {
			_mcc.Setup(x => x.GetList()).Returns(_dal.Mcc.GetList());

            var result = await _controller.GetList();

            Assert.AreEqual(0, result.Value.Count());
        }
    }
}