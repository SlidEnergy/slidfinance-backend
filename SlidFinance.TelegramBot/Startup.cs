using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlidFinance.TelegramBot.Models;

namespace SlidFinance.TelegramBot
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private IHostingEnvironment CurrentEnvironment { get; }

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			CurrentEnvironment = env;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			ConfigureBot(services);

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseMvc();

			//Bot Configurations
			//Bot.GetBotClientAsync().GetAwaiter().GetResult();


		}

		private void ConfigureBot(IServiceCollection services)
		{
			BotSettings botSettings;

			if (CurrentEnvironment.IsDevelopment())
			{
				botSettings = Configuration
					.GetSection("Security")
					.GetSection("Bot")
					.Get<BotSettings>();
			}
			else
			{
				botSettings = new BotSettings
				{
					Name = Environment.GetEnvironmentVariable("BOT_NAME"),
					Url = Environment.GetEnvironmentVariable("BOT_URL"),
					Key = Environment.GetEnvironmentVariable("BOT_KEY")
				};
			}

			//services.AddSingleton<BotSettings>(x => botSettings);
			services.AddSingleton<IBotService>(x => new BotService(botSettings));
			//	services.AddSingleton<IBotService>(x => {
			//	var service = new BotService(botSettings);
			//	service.InitAsync().GetAwaiter().GetResult();
			//	return service;
			//});
			services.AddSingleton<IUpdateService, UpdateService>();
		}
	}
}
