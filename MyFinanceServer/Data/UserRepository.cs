using MyFinanceServer.Models;
using System.Linq;

namespace MyFinanceServer.Data
{
    public class UserRepository : IUserRepository
    {
        ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Get(string username, string password)
        {
            return _dbContext.Users.Where(x => x.Email == username && x.Password == password).SingleOrDefault();
        }
    }
}
