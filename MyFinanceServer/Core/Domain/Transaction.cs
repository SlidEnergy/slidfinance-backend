﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Core
{
    public class Transaction
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public virtual BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        [Required]
        public string BankCategory { get; set; }

        public bool Approved { get; set; }
    }
}