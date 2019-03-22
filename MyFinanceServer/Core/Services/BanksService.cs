using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class BanksService
    {
        private IBanksRepository _repository;

        public BanksService(IBanksRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Bank>> GetList(string userId)
        {
            var categories = await _repository.GetListWithAccessCheck(userId);

            return categories.OrderBy(x => x.Title).ToList();
        }

        public async Task<Bank> AddBank(string userId, string title)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var bank = await _repository.Add<Bank>(new Bank(title, user));

            return bank;
        }

        public async Task<Bank> EditBank(string userId, int bankId, string title)
        {
            var editBank = await _repository.GetById(bankId);

            if (!editBank.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            editBank.Rename(title);

            await _repository.Update(editBank);

            return editBank;
        }

        public async Task DeleteBank(string userId, int bankId)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var bank = await _repository.GetById<int, Bank>(bankId);

            bank.IsBelongsTo(userId);

            await _repository.Delete<Bank>(bank);
        }
    }
}
