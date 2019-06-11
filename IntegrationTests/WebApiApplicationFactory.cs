using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyFinanceServer.Data;
using System;

namespace MyFinanceServer.IntegrationTests
{
	public class WebApiApplicationFactory<TStartup> : WebApplicationFactory<Startup>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Create a new service provider.
				var serviceProvider = new ServiceCollection()
					.AddEntityFrameworkInMemoryDatabase()
					.AddEntityFrameworkProxies()
					.BuildServiceProvider();

				// Add a database context (AppDbContext) using an in-memory database for testing.
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseInMemoryDatabase("InMemoryAppDb");
					options.UseLazyLoadingProxies();
					options.UseInternalServiceProvider(serviceProvider);
				});

				// Build the service provider.
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database contexts
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var appDb = scopedServices.GetRequiredService<ApplicationDbContext>();

					var logger = scopedServices.GetRequiredService<ILogger<WebApiApplicationFactory<TStartup>>>();
					// Ensure the database is created.
					appDb.Database.EnsureCreated();

					try
					{
						// Seed the database with some specific test data.
						SeedData.PopulateTestData(appDb);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "An error occurred seeding the " + "database with test messages. Error: {ex.Message}");
					}
				}
			});
		}
	}
}
