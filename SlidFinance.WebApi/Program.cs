using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SlidFinance.WebApi
{
	public class Program
    {
        public static void Main(string[] args)
        {
			// TODO: создавать роль администратора и пользователя с данной ролью по умолчанию
			// https://www.locktar.nl/programming/net-core/seed-database-users-roles-dotnet-core-2-0-ef/

			CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
