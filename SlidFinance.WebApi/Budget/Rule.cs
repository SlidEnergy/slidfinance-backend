using System;

namespace SlidFinance.WebApi.Dto
{
    public class Rule
    {
        public int Id { get; set; }

        public string Order { get; set; }

        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
