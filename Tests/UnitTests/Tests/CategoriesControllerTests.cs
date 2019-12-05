using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	[TestFixture]
    public class CategoriesControllerTests : TestsBase
    {
		private CategoriesController _controller;
		private Mock<ICategoriesService> _service;

        [SetUp]
        public void Setup()
        {
            _service = new Mock<ICategoriesService>();
			_controller = new CategoriesController(_autoMapper.Create(_db), _service.Object);
			_controller.AddControllerContext(_user);
		}

        [Test]
        public async Task GetCategories_ShouldBeListReturned()
        {
            var category1 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
            });
			var category2 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #2",
            });

			_service.Setup(x => x.GetListWithAccessCheckAsync(It.IsAny<string>())).ReturnsAsync(new List<Category>() { category1, category2 });

            var result = await _controller.GetList();

			_service.Verify(x => x.GetListWithAccessCheckAsync(It.Is<string>(u => u == _user.Id)));
		}
    }
}