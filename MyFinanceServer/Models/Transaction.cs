using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string BankDescription { get; set; }

        public string BankCategory { get; set; }
    }
}
