using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public Category Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Models.Account Account { get; set; }
    }
}
