﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using SlidFinance.TelegramBot.Models;
using SlidFinance.TelegramBot.Models.Commands;

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

			ConfigureDataAccess(services);

			services.AddSlidFinanceCore();

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
		}

		private void ConfigureDataAccess(IServiceCollection services)
		{
			services.AddEntityFrameworkNpgsql()
				.AddDbContext<ApplicationDbContext>(options => options
					.UseLazyLoadingProxies()
					.UseNpgsql(ConnectionStringFactory.Get()))
				.BuildServiceProvider();

			services.AddScoped<IRepository<ApplicationUser, string>, EfRepository<ApplicationUser, string>>();
			services.AddScoped<IRepositoryWithAccessCheck<Bank>, EfBanksRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Category>, EfCategoriesRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<BankAccount>, EfBankAccountsRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Rule>, EfRulesRepository>();
			services.AddScoped<IRepositoryWithAccessCheck<Transaction>, EfTransactionsRepository>();
			services.AddScoped<IRefreshTokensRepository, EfRefreshTokensRepository>();
			services.AddScoped<IRepository<Mcc, int>, EfRepository<Mcc, int>>();

			services.AddScoped<DataAccessLayer>();
		}

		private void ConfigureBot(IServiceCollection services)
		{
			TelegramBotSettings botSettings;

			if (CurrentEnvironment.IsDevelopment())
			{
				botSettings = Configuration
					.GetSection("Security")
					.GetSection("TelegramBot")
					.Get<TelegramBotSettings>();
			}
			else
			{
				botSettings = new TelegramBotSettings
				{
					Name = Environment.GetEnvironmentVariable("BOT_NAME"),
					Url = Environment.GetEnvironmentVariable("BOT_URL"),
					Token = Environment.GetEnvironmentVariable("BOT_TOKEN")
				};
			}

			services.AddSingleton<TelegramBotSettings>(x => botSettings);
			services.AddSingleton<IBotService>(x => new BotService(botSettings));
			services.AddScoped<IUpdateService, UpdateService>();

			services.AddScoped<CommandList>();
			services.AddScoped<GetCategoryStatisticCommand>();
			services.AddScoped<StartCommand>();
			services.AddScoped<WhichToPayCommand>();

			services.AddScoped<IMemoryCache>(x => new MemoryCache(new MemoryCacheOptions()));
		}
	}
}
