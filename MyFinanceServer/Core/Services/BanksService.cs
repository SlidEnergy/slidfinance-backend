using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class BanksService
    {
        private DataAccessLayer _dal;

        public BanksService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<List<Bank>> GetList(string userId)
        {
            var banks = await _dal.Banks.GetListWithAccessCheck(userId);

            return banks.OrderBy(x => x.Title).ToList();
        }

        public async Task<Bank> AddBank(string userId, string title)
        {
            var user = await _dal.Users.GetById(userId);

            var bank = await _dal.Banks.Add(new Bank(title, user));

            return bank;
        }

        public async Task<Bank> EditBank(string userId, int bankId, string title)
        {
            var editBank = await _dal.Banks.GetById(bankId);

            if (!editBank.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            editBank.Rename(title);

            await _dal.Banks.Update(editBank);

            return editBank;
        }

        public async Task DeleteBank(string userId, int bankId)
        {
            var user = await _dal.Users.GetById(userId);

            var bank = await _dal.Banks.GetById(bankId);

            bank.IsBelongsTo(userId);

            await _dal.Banks.Delete(bank);
        }
    }
}
