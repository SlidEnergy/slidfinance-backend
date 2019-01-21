using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class TransactionBindingModel
    {
        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }
    }
}
