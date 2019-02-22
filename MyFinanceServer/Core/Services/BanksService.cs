using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class BanksService
    {
        private IRepository _repository;

        public BanksService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Bank> AddBank(string userId, string title)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var bank = await _repository.Add<Bank>(new Bank(title, user));

            return bank;
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
