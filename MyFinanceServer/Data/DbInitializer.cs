using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MyFinanceServer.Data
{
    public class DbInitializer
    {
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context;

        public DbInitializer(IServiceProvider services)//, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task Initialize()
        {
            if ((await _userManager.FindByNameAsync("slidenergy@gmail.com")) != null)
                return;

            var user = new ApplicationUser()
            {
                UserName = "slidenergy@gmail.com",
                Email = "slidenergy@gmail.com"
            };
            await _userManager.CreateAsync(user, "Password123#");

            user.Banks = new Bank[]
            {
                new Bank
                {
                    Title = "HomeCreditBank", Accounts = new BankAccount[]
                    {
                        new BankAccount { Code ="homecredit_polza", Title = "Карта Польза", Balance = 0f}
                    }
                },
                new Bank
                {
                    Title = "RgsBank", Accounts = new BankAccount[]
                    {
                        new BankAccount { Code ="rgs_otlichnaya", Title = "Карта Отличная", Balance = 0f}
                    }
                },
                new Bank
                {
                    Title = "VostBank", Accounts = new BankAccount[]
                    {
                        new BankAccount { Code = "vost_common", Title = "Восточный банк: общий счет", Balance = 0f}
                    }
                },
                new Bank
                {
                    Title = "TinkoffBank", Accounts = new BankAccount[]
                    {
                        new BankAccount { Code = "tinkoff_black", Title = "Карта Тинькофф блэк", Balance = 0f}
                    }
                }
            };

            user.Categories = new Category[]
            {
                // Ежедневные траты
                new Category{Title = "Продукты"},
                new Category {Title = "Развлечения и спонтанные покупки"},
                new Category {Title = "Бензин"},
                new Category {Title = "Разное"},
                new Category {Title = "Настя разное"},
                new Category {Title = "Миша разное"},
                new Category {Title = "Аня разное"},

                // Фикс. месячные счета
                new Category {Title = "Быстринская (ипотека и комуналка)"},
                new Category {Title = "Телефон и интернет (мой, Настен, домашний)"},
                new Category {Title = "Спорт"},
                new Category {Title = "Столовка"},
                new Category {Title = "Разное (смс-банки)"},

                // Фикс. годовые счета
                new Category {Title = "Квартира (налог и страховка)"},
                new Category {Title = "Налоги ИП и фиксированные взносы"},
                new Category {Title = "Машина (налог и страховка)"},

                // Разовые платежи
                new Category {Title = "Машина"},
                new Category {Title = "Праздники и подарки"},
                new Category {Title = "Обучение"},
                new Category {Title = "Здоровье"},
                new Category {Title = "Путешествие"},

                // Студии
                new Category {Title = "ДБедного"},

                // Сбережения
                new Category {Title = "Курочка"}
            };

            await _context.SaveChangesAsync();
        }
    }
}
