using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public Bank Bank { get; set; }

        [Required]
        public ICollection<Models.Transaction> Transactions { get; set; }
    }
}
