using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api.Dto;
using MyFinanceServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("category")]
        public async Task<IEnumerable<CategoryStatistic>> GetCategoryStatistic(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.ToUniversalTime();
            endDate = endDate.ToUniversalTime();
            var userId = User.GetUserId();

            //var start = new DateTime(startDate.Year, startDate.Month, 1);
            //var end = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

            var groupedTransactions = await (from t in _context.Transactions
                    orderby t.DateTime descending 
                    where
                        t.DateTime >= startDate &&
                        t.DateTime <= endDate &&
                        t.Amount < 0 &&
                        t.Category != null &&
                        t.Account.Bank.User.Id == userId
                    group t by new
                    {
                        CategoryId = t.Category.Id,
                        Year = t.DateTime.Year,
                        Month = t.DateTime.Month
                    }
                    into g
                    select new
                    {
                        CategoryId = g.Key.CategoryId,
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Amount = g.Sum(x => x.Amount)
                    })
                .ToListAsync();

            var statistic = groupedTransactions
                .GroupBy(g2 => g2.CategoryId)
                .Select(x => new CategoryStatistic
                {
                    CategoryId = x.Key,
                    Months = x.Select(x1 => new MonthStatistic()
                    {
                        Month = new DateTime(x1.Year, x1.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                        Amount = x1.Amount
                    }).ToArray(),
                })
                .ToList();

            return statistic;
        }
    }
}