using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IMccService
	{
		Task<List<Mcc>> GetList();
	}
}