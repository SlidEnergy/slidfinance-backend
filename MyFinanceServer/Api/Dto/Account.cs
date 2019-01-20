using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api.Dto
{
    public class Account
    {
        public string Id { get; set; }

        public float Balance { get; set; }

        public string Title { get; set; }

        public string BankId { get; set; }

        public string[] TransactionIds { get; set; }
    }
}
