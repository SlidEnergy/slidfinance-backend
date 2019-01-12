using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class Account
    {
        public int Id { get; set; }

        public ICollection<Models.Transaction> Transactions { get; set; }
    }
}
