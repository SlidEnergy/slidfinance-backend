using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyFinanceServer.Data
{
    public class ApplicationDbContext : DbContext
    {
       // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //   => optionsBuilder.UseNpgsql("Host=localhost;Database=myfinance;Username=myfinanceuser;Password=myfinance");

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MyFinanceServer.Models.User> User { get; set; }
    }
}
