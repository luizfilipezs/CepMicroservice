using Microsoft.EntityFrameworkCore;
using CepMicroservice.Models;

namespace CepMicroservice.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().HasIndex(a => a.Cep).IsUnique();
        }
    }
}
