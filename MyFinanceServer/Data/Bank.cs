using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Data
{
    public class Bank
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public ICollection<BankAccount> Accounts { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}
