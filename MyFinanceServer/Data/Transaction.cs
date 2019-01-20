using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class Transaction
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public Category Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public BankAccount Account { get; set; }

        public int? Mcc { get; set; }

        [Required]
        public string BankCategory { get; set; }
    }
}
