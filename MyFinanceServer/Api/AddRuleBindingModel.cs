﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api
{
    public class AddRuleBindingModel
    {
        public int? AccountId { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
