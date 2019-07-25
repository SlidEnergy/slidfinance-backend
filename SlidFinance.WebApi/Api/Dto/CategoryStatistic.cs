using System;

namespace SlidFinance.WebApi.Dto
{
    public class CategoryStatistic
    {
        public int CategoryId { get; set; }

        public float AverageAmount { get; set; }

        public MonthStatistic[] Months { get; set; }
    }
}
