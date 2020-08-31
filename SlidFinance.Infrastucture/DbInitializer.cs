using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;

namespace SlidFinance.Infrastucture
{
	public class DbInitializer
	{
		public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
		{
			context.Database.EnsureCreated();

			await CreateDefaultUserAndRoleForApplication(context, userManager, roleManager, logger);
		}

		private static async Task CreateDefaultUserAndRoleForApplication(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
		{
			const string email = "admin";
			const string password = "admin";

			// Берем первого созданного пользователя
			var user = await context.Users.FirstOrDefaultAsync();

			// Для пустой базы создаем нового пользователя
			if (user == null)
			{
				user = await CreateDefaultUser(userManager, logger, email);

				// Временно отключаем проверку пароля для администратора
				var passwordValidators = userManager.PasswordValidators;
				userManager.PasswordValidators.Clear();

				await SetPasswordForUser(userManager, logger, email, user, password);

				// Возвращаем проверку пароля обратно
				foreach (var validator in passwordValidators)
					userManager.PasswordValidators.Add(validator);
			}

			var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == Role.Admin);

			// Создаем группу администратора, если в системе еще не было групп и устанавливаем её для пользователя
			// Если ранее был пользователь но не было группы, то для первого созданного пользователя будет установалена группа администратора
			if (role == null)
			{
				await CreateDefaultAdministratorRole(roleManager, logger, Role.Admin);

				await AddRoleToUser(userManager, logger, email, Role.Admin, user);
			}
		}

		private static async Task CreateDefaultAdministratorRole(RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger, string administratorRole)
		{
			logger.LogInformation($"Create the role `{administratorRole}` for application");
			var result = await roleManager.CreateAsync(new IdentityRole(administratorRole));
			if (result.Succeeded)
			{
				logger.LogDebug($"Created the role `{administratorRole}` successfully");
			}
			else
			{
				var exception = new ApplicationException($"Default role `{administratorRole}` cannot be created");
				logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(result));
				throw exception;
			}
		}

		private static async Task<ApplicationUser> CreateDefaultUser(UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger, string email)
		{
			logger.LogInformation($"Create default user with email `{email}` for application");
			var user = new ApplicationUser()
			{
				Email = email,
				UserName = email,
				Trustee = new Trustee()
			};

			var result = await userManager.CreateAsync(user);
			if (result.Succeeded)
			{
				logger.LogDebug($"Created default user `{email}` successfully");
			}
			else
			{
				var exception = new ApplicationException($"Default user `{email}` cannot be created");
				logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(result));
				throw exception;
			}

			var createdUser = await userManager.FindByEmailAsync(email);
			return createdUser;
		}

		private static async Task SetPasswordForUser(UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger, string email, ApplicationUser user, string password)
		{
			logger.LogInformation($"Set password for default user `{email}`");
			var result = await userManager.AddPasswordAsync(user, password);
			if (result.Succeeded)
			{
				logger.LogTrace($"Set password `{password}` for default user `{email}` successfully");
			}
			else
			{
				var exception = new ApplicationException($"Password for the user `{email}` cannot be set");
				logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(result));
				throw exception;
			}
		}

		private static async Task AddRoleToUser(UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger, string email, string administratorRole, ApplicationUser user)
		{
			logger.LogInformation($"Add default user `{email}` to role '{administratorRole}'");
			var result = await userManager.AddToRoleAsync(user, administratorRole);
			if (result.Succeeded)
			{
				logger.LogDebug($"Added the role '{administratorRole}' to default user `{email}` successfully");
			}
			else
			{
				var exception = new ApplicationException($"The role `{administratorRole}` cannot be set for the user `{email}`");
				logger.LogError(exception, GetIdentiryErrorsInCommaSeperatedList(result));
				throw exception;
			}
		}

		private static string GetIdentiryErrorsInCommaSeperatedList(IdentityResult result)
		{
			string errors = null;
			foreach (var identityError in result.Errors)
			{
				errors += identityError.Description;
				errors += ", ";
			}
			return errors;
		}
	}
}
