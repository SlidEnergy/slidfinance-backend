using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlidFinance.TelegramBot.Models;

namespace SlidFinance.TelegramBot
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			IWebHost webHost = CreateWebHostBuilder(args).Build();

			// Create a new scope
			using (var scope = webHost.Services.CreateScope())
			{
				var botService = scope.ServiceProvider.GetRequiredService<IBotService>();

				await botService.InitAsync();
			}

			// Run the WebHost, and start accepting requests
			// There's an async overload, so we may as well use it
			await webHost.RunAsync();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
