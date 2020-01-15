using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App.Analysis
{
	public interface ICashbackService
	{
		Task<List<WhichCardToPay>> WhichCardToPay(string userId, string searchString);
	}
}