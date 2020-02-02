using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SlidFinance.Infrastructure;

namespace SlidFinance.WebApi
{
	public class Program
    {
        public static void Main(string[] args)
        {
			// TODO: создавать роль администратора и пользователя с данной ролью по умолчанию
			// https://www.locktar.nl/programming/net-core/seed-database-users-roles-dotnet-core-2-0-ef/

            CreateHostBuilder(args)
				.Build()
				.MigrateDatabase<ApplicationDbContext>()
				.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
