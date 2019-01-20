using MyFinanceServer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Domain
{
    public interface IAccountDataSaver
    {
        Task Save(BankAccount account, float? accountBalance, ICollection<Transaction> transactions);
    }
}
