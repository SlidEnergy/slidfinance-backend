using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MyFinanceServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().HasData(
                new Models.User { Id = 1, Email = "slidenergy@gmail.com", Password = "slider123" });

            modelBuilder.Entity<Models.Bank>().HasData(
                new 
                {
                    Id = 1,
                    Title = "HomeCreditBank",
                    UserId = 1
                });

            modelBuilder.Entity<Models.Account>().HasData(
                new
                {
                    Id = 1,
                    BankId = 1,
                    Title = "Польза",
                    Balance = 0f
                });
        }

        public DbSet<Models.Transaction> Transactions { get; set; }

        public DbSet<Models.Account> Accounts { get; set; }

        public DbSet<Models.Bank> Banks { get; set; }
    }
}
