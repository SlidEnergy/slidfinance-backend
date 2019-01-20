using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api.Dto
{
    public class Account
    {
        public int Id { get; set; }

        public float Balance { get; set; }

        public string Title { get; set; }

        public int BankId { get; set; }

        public int[] TransactionIds { get; set; }
    }
}
