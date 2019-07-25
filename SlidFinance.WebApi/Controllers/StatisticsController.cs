using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlidFinance.WebApi.Dto;
using SlidFinance.Infrastucture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.App;

namespace SlidFinance.WebApi
{
    [Authorize]
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

            var start = startDate.AddMonths(-3);// new DateTime(startDate.Year, startDate.Month, 1);
            var end = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

            var groupedTransactions = await (from t in _context.Transactions
                                             orderby t.DateTime descending
                                             where
                                                 t.DateTime >= start &&
                                                 t.DateTime <= end &&
                                                 t.Category != null &&
                                                 t.Account.Bank.User.Id == userId
                                             group t by new
                                             {
                                                 CategoryId = t.Category.Id,
                                                 Year = t.DateTime.Year,
                                                 Month = t.DateTime.Month
                                             }
                                             into g
                                             select new GroupedTransactions
                                             {
                                                 CategoryId = g.Key.CategoryId,
                                                 Year = g.Key.Year,
                                                 Month = g.Key.Month,
                                                 Amount = g.Sum(x => x.Amount)
                                             })
                                             .ToListAsync();

            groupedTransactions = await GenerateAbsentRecords(groupedTransactions, start, end, userId);

            var statistic = groupedTransactions
                .GroupBy(g2 => g2.CategoryId)
                .Select(x => new CategoryStatistic
                {
                    CategoryId = x.Key,
                    AverageAmount = x.Average(x1 => x1.Amount),
                    Months = x
                        .Where(x1 => {
                            var date = new DateTime(x1.Year, x1.Month, 1, 0, 0, 0, DateTimeKind.Utc);

                            return date >= startDate && date <= endDate;
                        })
                        .Select(x1 => new MonthStatistic()
                        {
                            Month = new DateTime(x1.Year, x1.Month, 1, 0, 0, 0, DateTimeKind.Utc),
                            Amount = x1.Amount
                        }).ToArray(),
                })
                .ToList();

            return statistic;
        }

        private class GroupedTransactions {
            public int CategoryId;
            public int Year;
            public int Month;
            public float Amount;
        }

        private async Task<List<GroupedTransactions>> GenerateAbsentRecords(List<GroupedTransactions> transactions, DateTime startDate, DateTime endDate, string userId)
        {
            var list = new List<GroupedTransactions>();
            var categories = await _context.Categories.Where(x=>x.User.Id == userId).ToListAsync();

            DateTime currentDate = startDate;
            while (currentDate < endDate) {

                foreach (var category in categories)
                {
                    var t = transactions.FirstOrDefault(x => x.CategoryId == category.Id && x.Year == currentDate.Year && x.Month == currentDate.Month);

                    if (t != null)
                        list.Add(t);
                    else
                        list.Add(new GroupedTransactions { CategoryId = category.Id, Year = currentDate.Year, Month = currentDate.Month, Amount = 0f });
                }
                currentDate = currentDate.AddMonths(1);
            }

            return list;
        }
    }
}