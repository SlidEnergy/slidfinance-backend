using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api.Dto
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public DateTime DateTime { get; set; }

        public float Amount { get; set; }

        public int? CategoryId { get; set; }

        public string Description { get; set; }
    }
}
