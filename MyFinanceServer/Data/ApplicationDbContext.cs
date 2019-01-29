using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Data;

namespace MyFinanceServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<BankAccount> Accounts { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Rule> Rules { get; set; }
    }
}
