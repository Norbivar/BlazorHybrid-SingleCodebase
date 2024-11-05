using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Server.Database
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Id)
                .IsUnique();
        }
    }

    public class User : Shared.User
    {
        public string Password { get; set; }
    }
}
