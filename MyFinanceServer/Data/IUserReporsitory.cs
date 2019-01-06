using MyFinanceServer.Models;

namespace MyFinanceServer.Data
{
    public interface IUserRepository
    {
        User Get(string username, string password);
    }
}
