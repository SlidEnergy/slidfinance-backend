using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SlidFinance.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.WebApi
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
			ConfigureAuthorization(services);

			ConfigureAutoMapper(services);

			services.AddCors();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			ConfigureDataAccess(services);

			ConfigureSwagger(services);

			ConfigureApplicationServices(services);

			ConfigurePolicies(services);
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

			app.UseCors(x => x
			   .AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader());
			//.AllowCredentials());

			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseAuthentication();
			
			if (env.IsProduction())
			{ 
				app.UseHttpsRedirection();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(c =>
			{

			});

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseMvc();
		}

		private void ConfigureAuthorization(IServiceCollection services)
		{
			AuthSettings authSettings;

			if (CurrentEnvironment.IsDevelopment())
			{
				authSettings = Configuration
					.GetSection("Security")
					.GetSection("Token")
					.Get<AuthSettings>();
			}
			else
			{
				authSettings = new AuthSettings
				{
					Audience = Environment.GetEnvironmentVariable("TOKEN_AUDIENCE"),
					Issuer = Environment.GetEnvironmentVariable("TOKEN_ISSUER"),
					Key = Environment.GetEnvironmentVariable("TOKEN_KEY"),
					LifetimeMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("TOKEN_LIFETIME_MINUTES"))
				};
			}

			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						// Укзывает, будет ли проверяться издатель при проверке токена
						ValidateIssuer = false,
						// Строка, представляющая издателя
						ValidIssuer = authSettings.Issuer,

						// Будет ли проверяться потребитель токена
						ValidateAudience = false,
						// Установка потребителя токена
						ValidAudience = authSettings.Audience,
						// будет ли валидироваться время существования
						ValidateLifetime = true,

						// установка ключа безопасности
						IssuerSigningKey = authSettings.GetSymmetricSecurityKey(),
						// валидация ключа безопасности
						ValidateIssuerSigningKey = true,
					};
					// options.SaveToken = true;
				});

			services.AddSingleton<AuthSettings>(x => authSettings);

			// AddIdentity и AddDefaultIdentity добавляют много чего лишнего. Ссылки для сранения.
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Identity/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/UI/IdentityServiceCollectionUIExtensions.cs#L49
			services.AddIdentityCore<ApplicationUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
		}

		private void ConfigureAutoMapper(IServiceCollection services)
		{
			services.AddScoped(provider => new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfile(provider.GetService<ApplicationDbContext>()));
			}).CreateMapper());
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

			services.AddScoped<DataAccessLayer>();
		}

		private void ConfigureSwagger(IServiceCollection services)
		{
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "SlidFinance", Version = "v1" });
				c.AddSecurityDefinition("Oauth2", new OAuth2Scheme
				{
					Type = "oauth2",
					Flow = "password",
					TokenUrl = "/api/v1/users/token",
					Scopes = new Dictionary<string, string> {
						{ Policy.MustBeAllAccessMode, "Режим доступа: ко всем объектам" },
						{ Policy.MustBeAllOrImportAccessMode, "Режим доступа: ко всем объектам или только импорт" }
					}
				});
				c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

				c.DescribeAllEnumsAsStrings();

				c.OperationFilter<ResponseWithDescriptionOperationFilter>();
				c.OperationFilter<SecurityRequirementsOperationFilter>();

				c.SchemaFilter<EnumAsModelSchemaFilter>();
			});
		}

		private void ConfigureApplicationServices(IServiceCollection services)
		{
			services.AddScoped<ITokenGenerator, TokenGenerator>();
			services.AddScoped<AccountsService>();
			services.AddScoped<BanksService>();
			services.AddScoped<RulesService>();
			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<CategoriesService>();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<TransactionsService>();
			services.AddScoped<IImportService, ImportService>();
		}

		private void ConfigurePolicies(IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				// Добавляем политики на наличие нужной роли у учётной записи.
				options.AddPolicy(Policy.MustBeAllAccessMode, policy => policy.RequireClaim(nameof(AccessMode), AccessMode.All.ToString()));
				options.AddPolicy(Policy.MustBeAllOrImportAccessMode, policy => policy.RequireClaim(nameof(AccessMode), AccessMode.All.ToString(), AccessMode.Import.ToString()));
			});
		}
	}
}
