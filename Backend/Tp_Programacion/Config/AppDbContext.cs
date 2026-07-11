using Microsoft.EntityFrameworkCore;
using Tp_Programacion.Models.Curso;
using Tp_Programacion.Models.Role;
using Tp_Programacion.Models.User;

namespace Tp_Programacion.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Curso> Cursos { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
    		modelBuilder.Entity<User>()
        	.HasMany(u => u.Roles)
        	.WithMany(); // 
    	modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
    	modelBuilder.Entity<Role>().HasData(
        	new Role() { Id = 1, Name = "Admin" },
        	new Role() { Id = 2, Name = "UserGratis" },
        	new Role() { Id = 3, Name = "UserPremium" }
    	);
    	modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
    	modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
	}
    }
}
