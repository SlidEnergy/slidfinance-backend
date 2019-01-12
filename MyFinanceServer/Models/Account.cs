using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Models
{
    public class Account
    {
        public int Id { get; set; }

        public float Balance { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public Bank Bank { get; set; }

        [Required]
        public ICollection<Models.Transaction> Transactions { get; set; }
    }
}
