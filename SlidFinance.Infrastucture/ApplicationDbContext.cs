using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlidFinance.App;
using SlidFinance.Domain;

namespace SlidFinance.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TrusteeAccount>()
				.HasKey(key => new { key.AccountId, key.TrusteeId});

			modelBuilder.Entity<TrusteeCategory>()
				.HasKey(key => new { key.CategoryId, key.TrusteeId });
		}

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<BankAccount> Accounts { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<AuthToken> AuthTokens { get; set; }

        public DbSet<Mcc> Mcc { get; set; }

        public DbSet<Models.Merchant> Merchants { get; set; }

		public DbSet<TrusteeAccount> TrusteeAccounts { get; set; }
		public DbSet<TrusteeCategory> TrusteeCategories { get; set; }

		public DbSet<ApplicationUser> Users { get; set; }
    }
}
