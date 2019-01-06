using Microsoft.EntityFrameworkCore;

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
        }
    }
}
