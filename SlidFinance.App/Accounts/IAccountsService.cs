using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IAccountsService
	{
		Task<BankAccount> AddAccount(string userId, int bankId, string title, string code, float balance, float creditLimit);
		Task DeleteAccount(string userId, int accountId);
		Task<BankAccount> EditAccount(string userId, int accountId, string title, string code, float balance, float creditLimit);
		Task<BankAccount> GetByIdWithAccessCheck(string userId, int id);
		Task<List<BankAccount>> GetListWithAccessCheckAsync(string userId, int? bankId = null);
		Task<BankAccount> PatchAccount(string userId, BankAccount account);
	}
}