using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class CategoryStatisticService : ICategoryStatisticService
	{
		private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public CategoryStatisticService(DataAccessLayer dal, IApplicationDbContext context)
		{
			_dal = dal;
			_context = context;
		}

		public async Task<List<CategoryStatistic>> GetStatistic(string userId, DateTime startDate, DateTime endDate)
		{
			var start = startDate.AddMonths(-3);// new DateTime(startDate.Year, startDate.Month, 1);
			var end = new DateTime(endDate.Year, endDate.Month, DateTime.DaysInMonth(endDate.Year, endDate.Month));

			var transactions = await _context.GetTransactionListWithAccessCheckAsync(userId);

			var groupedTransactions = (from t in transactions.AsQueryable()
									   orderby t.DateTime descending
									   where
										   t.DateTime >= start &&
										   t.DateTime <= end &&
										   t.Category != null
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
											 .ToList();

			groupedTransactions = await GenerateAbsentRecords(groupedTransactions, start, end, userId);

			var statistic = groupedTransactions
				.GroupBy(g2 => g2.CategoryId)
				.Select(x => new CategoryStatistic
				{
					CategoryId = x.Key,
					AverageAmount = x.Average(x1 => x1.Amount),
					Months = x
						.Where(x1 =>
						{
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

		private class GroupedTransactions
		{
			public int CategoryId;
			public int Year;
			public int Month;
			public float Amount;
		}

		private async Task<List<GroupedTransactions>> GenerateAbsentRecords(List<GroupedTransactions> transactions, DateTime startDate, DateTime endDate, string userId)
		{
			var list = new List<GroupedTransactions>();
			var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

			DateTime currentDate = startDate;
			while (currentDate < endDate)
			{

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
