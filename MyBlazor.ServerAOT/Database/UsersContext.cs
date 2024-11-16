using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Server.Database
{
    public class UsersContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
        //public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "blazortest");

				entity.HasIndex(b => b.Id)
	                .IsUnique();
			});
        }
    }

    public class User : IdentityUser<int>
	{
		public string Name { get; set; }
    }
}
