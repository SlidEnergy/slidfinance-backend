using MyFinanceServer.Data;

namespace MyFinanceServer.Api
{
    public interface ITokenGenerator
    {
        string Get(ApplicationUser user);
    }
}
