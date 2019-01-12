using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer
{
    public interface ITokenGenerator
    {
        string Get(string username);
    }
}
