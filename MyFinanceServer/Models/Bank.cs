using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Bank
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public ICollection<Models.Account> Accounts { get; set; }

        [Required]
        public User User { get; set; }
    }
}
