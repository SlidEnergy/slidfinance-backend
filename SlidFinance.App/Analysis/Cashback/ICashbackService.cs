using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App.Analysis
{
	public interface ICashbackService
	{
		Task<List<WhichCardToPay>> WhichCardToPayAsync(string userId, string searchString);
	}
}