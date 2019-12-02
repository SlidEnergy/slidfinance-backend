using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class MccService : IMccService
	{
		private DataAccessLayer _dal;

		public MccService(DataAccessLayer dal)
		{
			_dal = dal;
		}

		public async Task<List<Mcc>> GetList()
		{
			var mcc = await _dal.Mcc.GetList();

			return mcc.ToList();
		}
	}
}
