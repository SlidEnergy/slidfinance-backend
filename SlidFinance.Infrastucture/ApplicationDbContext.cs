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

			modelBuilder.Entity<TrusteeProduct>()
				.HasKey(key => new { key.ProductId, key.TrusteeId });

            modelBuilder.Entity<CashbackCategoryMcc>()
                .HasOne(x => x.Category)
                .WithMany()
                .IsRequired();


            modelBuilder.Entity<ProductTariff>()
                .HasOne(x => x.Product)
                .WithMany()
                .IsRequired();

            modelBuilder.Entity<CashbackCategory>()
             .HasOne(x => x.Tariff)
             .WithMany()
             .IsRequired();

            modelBuilder.Entity<Merchant>()
             .HasOne(x => x.Mcc)
             .WithMany()
             .IsRequired();

            modelBuilder.Entity<Merchant>()
             .HasOne(x => x.CreatedBy)
             .WithMany()
             .IsRequired();
        }

		public DbSet<Transaction> Transactions { get; set; }

        public DbSet<BankAccount> Accounts { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<UserCategory> Categories { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<AuthToken> AuthTokens { get; set; }

        public DbSet<Mcc> Mcc { get; set; }

        public DbSet<Merchant> Merchants { get; set; }

		public DbSet<TrusteeAccount> TrusteeAccounts { get; set; }
		public DbSet<TrusteeCategory> TrusteeCategories { get; set; }
		public DbSet<TrusteeProduct> TrusteeProducts { get; set; }

		public DbSet<ApplicationUser> Users { get; set; }

		public DbSet<Product> Products { get; set; }

        public DbSet<ProductTariff> Tariffs { get; set; }

        public DbSet<CashbackCategory> CashbackCategories { get; set; }

        public DbSet<Cashback> Cashback { get; set; }

        public DbSet<CashbackCategoryMcc> CashbackCategoryMcc { get; set; }
    }
}
