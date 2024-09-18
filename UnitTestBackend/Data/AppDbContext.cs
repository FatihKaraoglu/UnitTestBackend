using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UnitTestBackend.Entity;
using UnitTestShared.Entity;

namespace UnitTestBackend.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Icemachine 5K Plus", Price = 9.99m, Category = "Electronics" },
                new Product { Id = 2, Name = "Air Fryer Super Hot", Price = 19.99m, Category = "Electronics" }
            );
        }
    }
}
