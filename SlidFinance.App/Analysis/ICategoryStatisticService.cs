using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface ICategoryStatisticService
	{
		Task<List<CategoryStatistic>> GetStatistic(string userId, DateTime startDate, DateTime endDate);
	}
}