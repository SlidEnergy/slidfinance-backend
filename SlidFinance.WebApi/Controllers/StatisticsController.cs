using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Authorize(Policy = Policy.MustBeAllAccessMode)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ICategoryStatisticService _service;

        public StatisticsController(ICategoryStatisticService service)
        {
            _service = service;
        }

        [HttpGet("category")]
        public async Task<IEnumerable<CategoryStatistic>> GetCategoryStatistic(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();
            var userId = User.GetUserId();

			return await _service.GetStatistic(userId, startDate, endDate);
        }
    }
}