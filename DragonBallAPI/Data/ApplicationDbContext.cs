using Microsoft.EntityFrameworkCore;
using DragonBallAPI.Models;

namespace DragonBallAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Transformation> Transformations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la relación entre Character y Transformation
            modelBuilder.Entity<Transformation>()
                .HasOne(t => t.Character)
                .WithMany(c => c.Transformations)
                .HasForeignKey(t => t.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sembrar algunos usuarios para autenticación
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "admin123" },
                new User { Id = 2, Username = "user", Password = "user123" }
            );
        }
    }
}