using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyFinanceServer
{
    public class AuthOptions
    {
        public const string ISSUER = "MyFinanceServer"; // издатель токена
        public const string AUDIENCE = "myfinance.com"; // потребитель токена
        const string KEY = "aofTiZ5JCmGKTATOSOB78xeu4lqzaczj";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
