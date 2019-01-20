using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinanceServer.Api.Dto
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public int[] BankIds { get; set; }

        public int[] CategoryIds { get; set; }
    }
}
