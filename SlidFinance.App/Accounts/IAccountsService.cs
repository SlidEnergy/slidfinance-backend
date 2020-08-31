using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IAccountsService
	{
		Task<BankAccount> AddAccount(string userId, BankAccount account);
		Task DeleteAccount(string userId, int accountId);
		Task<BankAccount> Update(string userId, BankAccount account);
		Task<BankAccount> GetByIdWithAccessCheckAsync(string userId, int id);
		Task<List<BankAccount>> GetListWithAccessCheckAsync(string userId, int? bankId = null);
		Task<BankAccount> PatchAccount(string userId, BankAccount account);
	}
}