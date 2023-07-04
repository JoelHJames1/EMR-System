using EMRDataLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EMRDataLayer.DataContext
{
    public class EMRDBContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public EMRDBContext(DbContextOptions<EMRDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure additional entity configurations here
            modelBuilder.Entity<Address>().HasKey(a => a.Id);
            // Configure keys and relationships for other entities here
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
