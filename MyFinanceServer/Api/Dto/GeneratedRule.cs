using System;

namespace MyFinanceServer.Api.Dto
{
    public class GeneratedRule
    {
        public int? AccountId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }

        public CategoryDistribution[] Categories { get; set; }

        public int Count { get; set; }
    }
}
