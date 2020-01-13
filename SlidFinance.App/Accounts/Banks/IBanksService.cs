using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IBanksService
	{
		Task<Bank> AddBank(string title);
		Task DeleteBank(int bankId);
		Task<Bank> EditBank(int bankId, string title);
		Task<List<Bank>> GetLis();
	}
}