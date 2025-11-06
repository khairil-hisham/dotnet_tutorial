using Microsoft.EntityFrameworkCore;
using AuthApi.Models;

namespace AuthApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .HasColumnType("BLOB");

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordSalt)
                .HasColumnType("BLOB");
        }
    }
}