using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class AddRuleBindingModel
    {
        public string AccountId { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
