using AutoDealer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDealer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<Transmission> Transmissions { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aici putem pune date de seed sau configurari extra daca e nevoie
            base.OnModelCreating(modelBuilder);
        }
    }
}