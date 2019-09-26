using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;

namespace SlidFinance.Infrastructure
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

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<BankAccount> Accounts { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
