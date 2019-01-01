using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
    }
}
