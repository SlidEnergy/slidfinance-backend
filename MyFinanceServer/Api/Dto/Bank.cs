using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Api.Dto
{
    public class Bank
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int[] AccountIds { get; set; }

        public float OwnFunds { get; set; }
    }
}
