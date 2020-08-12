using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using SlidFinance.Infrastucture;
using System.Linq;
using System.Collections.Generic;

namespace SlidFinance.WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args)
				.Build()
				.MigrateDatabase<ApplicationDbContext>();

			InitializeDb(host);

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		private static void InitializeDb(IHost host)
		{
			// Создаум роль администратора и пользователя с данной ролью по умолчанию
			// https://www.locktar.nl/programming/net-core/seed-database-users-roles-dotnet-core-2-0-ef/

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var context = services.GetRequiredService<ApplicationDbContext>();
					var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
					var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
					var dbInitializerLogger = services.GetRequiredService<ILogger<DbInitializer>>();
					DbInitializer.Initialize(context, userManager, roleManager, dbInitializerLogger).Wait();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}
		}
	}
}
