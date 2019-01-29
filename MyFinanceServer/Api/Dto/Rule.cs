using System;

namespace MyFinanceServer.Api.Dto
{
    public class Rule
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
