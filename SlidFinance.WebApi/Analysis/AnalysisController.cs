using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.App.Analysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Authorize(Policy = Policy.MustBeAllOrExportAccessMode)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ICashbackService _cashbackService;
        private readonly ICategoryStatisticService _categoryStatisticsService;

        public AnalysisController(ICategoryStatisticService categoryStatisticsService, ICashbackService cashbackService)
        {
            _cashbackService = cashbackService;
            _categoryStatisticsService = categoryStatisticsService;
        }

        [HttpGet("whichcardtopay")]
        public async Task<IEnumerable<WhichCardToPay>> WhichCardToPay(string search)
        {
            var userId = User.GetUserId();

			return await _cashbackService.WhichCardToPayAsync(userId, search);
        }

        [HttpGet("category")]
        public async Task<IEnumerable<CategoryStatistic>> GetCategoryStatistic(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();
            var userId = User.GetUserId();

            return await _categoryStatisticsService.GetStatistic(userId, startDate, endDate);
        }
    }
}