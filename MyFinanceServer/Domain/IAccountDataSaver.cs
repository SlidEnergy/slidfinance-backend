using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Domain
{
    public interface IAccountDataSaver
    {
        Task Save(Models.Account account, float? accountBalance, ICollection<Models.Transaction> transactions);
    }
}
