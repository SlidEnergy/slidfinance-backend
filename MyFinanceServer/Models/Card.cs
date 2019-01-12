using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Card : Account
    {
        [Required]
        public string Number { get; set; }
    }
}
