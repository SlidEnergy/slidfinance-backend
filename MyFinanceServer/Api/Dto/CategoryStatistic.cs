using System;

namespace MyFinanceServer.Api.Dto
{
    public class CategoryStatistic
    {
        public int CategoryId { get; set; }

        public MonthStatistic[] Months { get; set; }
        
        public float[] Amounts { get; set; }
    }
}
