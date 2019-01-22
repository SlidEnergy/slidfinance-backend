using System;

namespace MyFinanceServer.Api.Dto
{
    public class CategoryStatistic
    {
        public string CategoryId { get; set; }

        public MonthStatistic[] Months { get; set; }
        
        public float[] Amounts { get; set; }
    }
}
