using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.Domain
{
    public interface IAccountDataSaver
    {
        Task Save(string userId, BankAccount account, float? accountBalance, ICollection<Transaction> transactions);
    }
}
