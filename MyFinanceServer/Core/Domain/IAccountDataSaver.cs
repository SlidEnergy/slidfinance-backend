using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public interface IAccountDataSaver
    {
        Task Save(string userId, BankAccount account, float? accountBalance, ICollection<Transaction> transactions);
    }
}
