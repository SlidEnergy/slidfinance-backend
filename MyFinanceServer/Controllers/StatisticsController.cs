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
        public async Task<IEnumerable<MonthStatistic>> GetCategoryStatistic(int year, int month)
        {
            var userId = User.GetUserId();

            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            return await (from t in _context.Transactions
                where
                    t.DateTime >= startDate &&
                    t.DateTime <= endDate &&
                    t.Account.Bank.User.Id == userId
                group t by t.Category.Id
                into g
                select new MonthStatistic {CategoryId = g.Key, Amount = g.Sum(x => x.Amount)}).ToListAsync();
        }
    }
}