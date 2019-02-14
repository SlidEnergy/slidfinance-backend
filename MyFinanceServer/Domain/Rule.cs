﻿using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class Rule
    {
        public string Id { get; set; }

        public int Order { get; set; }

        [Required]
        public virtual Category Category { get; set; }

        public string Description { get; set; }

        public virtual BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        public string BankCategory { get; set; }
    }
}
