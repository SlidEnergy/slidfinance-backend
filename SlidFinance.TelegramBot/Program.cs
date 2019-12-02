using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

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
