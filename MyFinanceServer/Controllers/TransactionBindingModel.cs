using System;

namespace MyFinanceServer.Api
{
    public class TransactionBindingModel
    {
        public int AccountId { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }
    }
}
