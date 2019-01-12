using MyFinanceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public interface ITransactionRepository
    {
        Task Add(Transaction transaction);

        Task<IEnumerable<Models.Transaction>> GetList();
    }
}
