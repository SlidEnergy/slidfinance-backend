using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyFinanceServer.Models;

namespace MyFinanceServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().HasData(
                new Models.User { Id = 1, Email = "slidenergy@gmail.com", Password = "slider123" });

            modelBuilder.Entity<Models.Bank>().HasData(
                new { Id = 1, Title = "HomeCreditBank", UserId = 1 },
                new { Id = 2, Title = "RgsBank", UserId = 1 },
                new { Id = 3, Title = "VostBank", UserId = 1 },
                new { Id = 4, Title = "TinkoffBank", UserId = 1 }
            );

            modelBuilder.Entity<Models.Account>().HasData(
                new { Id = 1, BankId = 1, Title = "Карта Польза", Balance = 0f },
                new { Id = 2, BankId = 2, Title = "Карта Отличная", Balance = 0f },
                new { Id = 3, BankId = 3, Title = "Общий счет", Balance = 0f },
                new { Id = 4, BankId = 4, Title = "Карта Тинькофф блэк", Balance = 0f });

            modelBuilder.Entity<Models.Category>().HasData(
                // Ежедневные траты
                new { Id = 1, Title = "Продукты", UserId = 1},
                new { Id = 2, Title = "Развлечения и спонтанные покупки", UserId = 1 },
                new { Id = 3, Title = "Бензин", UserId = 1 },
                new { Id = 4, Title = "Разное", UserId = 1 },
                new { Id = 5, Title = "Настя разное", UserId = 1 },
                new { Id = 6, Title = "Миша разное", UserId = 1 },
                new { Id = 7, Title = "Аня разное", UserId = 1 },

                // Фикс. месячные счета
                new { Id = 8, Title = "Быстринская (ипотека и комуналка)", UserId = 1 },
                new { Id = 9, Title = "Телефон и интернет (мой, Настен, домашний)", UserId = 1 },
                new { Id = 10, Title = "Спорт", UserId = 1 },
                new { Id = 11, Title = "Столовка", UserId = 1 },
                new { Id = 12, Title = "Разное (смс-банки)", UserId = 1 },
                
                // Фикс. годовые счета
                new { Id = 13, Title = "Квартира (налог и страховка)", UserId = 1 },
                new { Id = 14, Title = "Налоги ИП и фиксированные взносы", UserId = 1 },
                new { Id = 15, Title = "Машина (налог и страховка)", UserId = 1 },

                // Разовые платежи
                new { Id = 16, Title = "Машина", UserId = 1 },
                new { Id = 17, Title = "Праздники и подарки", UserId = 1 },
                new { Id = 18, Title = "Обучение", UserId = 1 },
                new { Id = 19, Title = "Здоровье", UserId = 1 },
                new { Id = 20, Title = "Путешествие", UserId = 1 },

                // Студии
                new { Id = 21, Title = "ДБедного", UserId = 1 },

                // Сбережения
                new { Id = 22, Title = "Курочка", UserId = 1 }
            );
        }

        public DbSet<Models.User> Users { get; set; }

        public DbSet<Models.Transaction> Transactions { get; set; }

        public DbSet<Models.Account> Accounts { get; set; }

        public DbSet<Models.Bank> Banks { get; set; }

        public DbSet<MyFinanceServer.Models.Category> Category { get; set; }
    }
}
