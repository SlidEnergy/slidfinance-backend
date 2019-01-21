﻿using System;

namespace MyFinanceServer.Api.Dto
{
    public class Transaction
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
