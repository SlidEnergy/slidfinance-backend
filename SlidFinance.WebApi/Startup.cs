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
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Newtonsoft;
using Microsoft.AspNetCore.Mvc.Controllers;
using AspNetCore.Authentication.ApiKey;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SlidFinance.WebApi.Saltedge;
using SlidFinance.App.Saltedge;
using SaltEdgeNetCore;

namespace SlidFinance.WebApi
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private IWebHostEnvironment CurrentEnvironment { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
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
			services.AddControllers()
				.AddNewtonsoftJson(opts =>
				{
					opts.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
					opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				});


			ConfigureInfrastructure(services);

			ConfigureSwagger(services);

			ConfigureTelegramBot(services);
			ConfigureApplicationServices(services);

			ConfigurePolicies(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseCors(x => x
			   .AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader());
			//.AllowCredentials());

			app.UseAuthentication();
			app.UseAuthorization();

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

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
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

			services.AddSingleton<AuthSettings>(x => authSettings);

			// AddIdentity и AddDefaultIdentity добавляют много чего лишнего. Ссылки для сранения.
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Identity/IdentityServiceCollectionExtensions.cs
			// https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/UI/IdentityServiceCollectionUIExtensions.cs#L49
			services.AddIdentityCore<ApplicationUser>(options =>
			{
				options.User.RequireUniqueEmail = true;

				// Задаем ClaimType которые будут записываться в токен, при восстановлении токена, эти параметры не учитываются
				options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
				options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Email;
				options.ClaimsIdentity.RoleClaimType = "role";
			})
				.AddRoles<IdentityRole>()
				.AddRoleManager<RoleManager<IdentityRole>>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddSlidFinanceCore();

			services
				.AddAuthentication(sharedOptions =>
				{
					sharedOptions.DefaultScheme = "smart";
					sharedOptions.DefaultChallengeScheme = "smart";
				})
				.AddPolicyScheme("smart", "Authorization Bearer or api key", options =>
				{
					options.ForwardDefaultSelector = context =>
					{
						var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
						if (authHeader?.StartsWith("Bearer ") == true)
						{
							return JwtBearerDefaults.AuthenticationScheme;
						}
						return ApiKeyDefaults.AuthenticationScheme;
					};
				})
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
				})
				.AddApiKeyInQueryParams<Auth.ApiKeyProvider>(options =>
				{
					options.Realm = "SlidFinance";
					options.KeyName = "api_key";
				});
		}

		private void ConfigureAutoMapper(IServiceCollection services)
		{
			services.AddScoped(provider => new MapperConfiguration(cfg =>
			{
				cfg.AddProfile(new MappingProfile(provider.GetService<ApplicationDbContext>()));
			}).CreateMapper());
		}

		private void ConfigureSwagger(IServiceCollection services)
		{
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "SlidFinance", Version = "v1" });

				c.AddSecurityDefinition("Oauth2", new OpenApiSecurityScheme
				{
					Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
					Flows = new OpenApiOAuthFlows
					{
						Implicit = new OpenApiOAuthFlow
						{
							AuthorizationUrl = new Uri("/api/v1/users/token", UriKind.Relative),
							Scopes = new Dictionary<string, string>
							{
								{ Policy.MustBeAllAccessMode, "Режим доступа: ко всем объектам" },
								{ Policy.MustBeAllOrImportAccessMode, "Режим доступа: ко всем объектам или только импорт" }
							}
						}
					}
				});

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "Токен авторизации JWT, использующий схему Bearer. Пример: \"Authorization: Bearer {token}\", provide value: \"Bearer {token}\"",
					Name = "Authorization",
					In = Microsoft.OpenApi.Models.ParameterLocation.Header,
					Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
				});


				c.OperationFilter<ResponseWithDescriptionOperationFilter>();
				c.OperationFilter<SecurityRequirementsOperationFilter>();

				c.CustomOperationIds(apiDesc =>
				{
					if (apiDesc.ActionDescriptor is ControllerActionDescriptor)
					{
						var descriptor = (ControllerActionDescriptor)apiDesc.ActionDescriptor;
						return $"{descriptor.ControllerName}_{descriptor.ActionName}";
					}

					return null;
				});
			});

			services.AddSwaggerGenNewtonsoftSupport();
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

		private void ConfigureApplicationServices(IServiceCollection services)
		{
			services.AddScoped<IClaimsGenerator, ClaimsGenerator>();
			services.AddScoped<ITokenGenerator, TokenGenerator>();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<ITelegramService, TelegramService>();
			services.AddScoped<IApiImportService, ApiImportService>();

			services.AddSlidFinanceCore();
		}

		private void ConfigurePolicies(IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				// Добавляем политики на наличие нужной роли у учётной записи.
				options.AddPolicy(Policy.MustBeAllAccessMode, policy => policy.RequireClaim(nameof(AccessMode), AccessMode.All.ToString()));
				options.AddPolicy(Policy.MustBeAllOrImportAccessMode, policy => policy.RequireClaim(nameof(AccessMode), AccessMode.All.ToString(), AccessMode.Import.ToString()));
				options.AddPolicy(Policy.MustBeAllOrExportAccessMode, policy => policy.RequireClaim(nameof(AccessMode), AccessMode.All.ToString(), AccessMode.Export.ToString()));
				options.AddPolicy(Policy.MustBeAdmin, policy => policy.RequireUserName("slidenergy@gmail.com"));
			});
		}

		private void ConfigureTelegramBot(IServiceCollection services)
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
					Name = Environment.GetEnvironmentVariable("TELEGRAM_BOT_NAME"),
					Token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
				};
			}

			services.AddSingleton<TelegramBotSettings>(x => botSettings);
		}

		private void ConfigureSaltedge(IServiceCollection services)
		{
			SaltedgeSettings saltedgeSettings;

			if (CurrentEnvironment.IsDevelopment())
			{
				saltedgeSettings = Configuration
					.GetSection("Security")
					.GetSection("Saltedge")
					.Get<SaltedgeSettings>();
			}
			else
			{
				saltedgeSettings = new SaltedgeSettings
				{
					AppId = Environment.GetEnvironmentVariable("SALTEDGE_APP_ID"),
					Secret = Environment.GetEnvironmentVariable("SALTEDGE_SECRET"),
				};
			}

			services.AddSaltEdge(options =>
			{
				options.AppId = saltedgeSettings.AppId;
				options.Secret = saltedgeSettings.Secret;
				options.LiveMode = false;
			});

			services.AddSingleton<SaltedgeSettings>(x => saltedgeSettings);
			services.AddScoped<ISaltedgeService, SaltedgeService>();
		}
	}
}
