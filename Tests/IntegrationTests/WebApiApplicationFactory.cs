﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlidFinance.Infrastructure;
using System;
using System.Linq;

namespace SlidFinance.WebApi.IntegrationTests
{
	public class WebApiApplicationFactory<TStartup> : WebApplicationFactory<Startup>
	{
		public Action<IServiceCollection> Registrations { get; set; }

		public WebApiApplicationFactory() : this(null)
		{
		}

		public WebApiApplicationFactory(Action<IServiceCollection> registrations = null)
		{
			Registrations = registrations ?? (collection => { });
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestServices(services =>
			{
				// Remove the app's ApplicationDbContext registration.
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType ==
						typeof(DbContextOptions<ApplicationDbContext>));

				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

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
