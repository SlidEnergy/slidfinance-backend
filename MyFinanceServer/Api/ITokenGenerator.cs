using MyFinanceServer.Core;

namespace MyFinanceServer.Api
{
    public interface ITokenGenerator
    {
        string Get(ApplicationUser user);
    }
}
