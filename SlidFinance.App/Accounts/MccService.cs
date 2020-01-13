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

		public async Task<List<Mcc>> GetListAsync()
		{
			var mcc = await _dal.Mcc.GetList();

			return mcc.ToList();
		}

		public async Task<Mcc> GetByIdAsync(int id)
		{
			return await _dal.Mcc.GetById(id);
		}

		public async Task<Mcc> AddAsync(Mcc mcc)
		{
			await _dal.Mcc.Add(mcc);

			return mcc;
		}
	}
}
