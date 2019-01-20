using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class BankAccount
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public float Balance { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Bank Bank { get; set; }

        [Required]
        public ICollection<Transaction> Transactions { get; set; }
    }
}
