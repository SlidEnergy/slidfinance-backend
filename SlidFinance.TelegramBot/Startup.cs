﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using SlidFinance.TelegramBot.Bots;
using SlidFinance.TelegramBot.Dialogs;
using SlidFinance.TelegramBot.Models;
using System;

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
			// AddIdentity и AddDefaultIdentity добавляют много чего лишнего. Ссылки для сранения.
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Identity/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/UI/IdentityServiceCollectionUIExtensions.cs#L49
			services.AddIdentityCore<ApplicationUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			ConfigureInfrastructure(services);
			services.AddSlidFinanceCore();
			services.AddMvc(opt =>
            {
				opt.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			ConfigureBot(services);
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

			//app.UseWebSockets();
			//app.UseBotFramework();
		}

		private void ConfigureInfrastructure(IServiceCollection services)
		{
			string connectionString = "";

			if (CurrentEnvironment.IsProduction())
				connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
			else
				connectionString = Configuration.GetConnectionString(Environment.MachineName) ??
					Configuration.GetConnectionString("DefaultConnection");

			services.AddSlidFinanceInfrastructure(connectionString);
		}

		private void ConfigureBot(IServiceCollection services)
		{
			BotBuilderSettings botBuilderSettings;

			if (CurrentEnvironment.IsDevelopment())
			{
				botBuilderSettings = Configuration
					.GetSection("Security")
					.GetSection("BotBuilder")
					.Get<BotBuilderSettings>();
			}
			else
			{
				botBuilderSettings = new BotBuilderSettings
				{
					MicrosoftAppId = Environment.GetEnvironmentVariable("MicrosoftAppId"),
					MicrosoftAppPassword = Environment.GetEnvironmentVariable("MicrosoftAppPassword"),
				};
			}

			services.AddSingleton<ICredentialProvider>(x => new SimpleCredentialProvider(botBuilderSettings.MicrosoftAppId, botBuilderSettings.MicrosoftAppPassword));

			// Create the Bot Framework Adapter with error handling enabled.
			services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

			// Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
			services.AddSingleton<IStorage, MemoryStorage>();

			// Create the User state. (Used in this bot's Dialog implementation.)
			services.AddSingleton<UserState>();

			// Create the Conversation state. (Used by the Dialog system itself.)
			services.AddSingleton<ConversationState>();

			services.AddScoped<WhichCardToPayDialog>();
			services.AddScoped<CategoryStatisticDialog>();

			// The MainDialog that will be run by the bot.
			services.AddScoped<MainDialog>();

			// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
			services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
		}
	}
}
