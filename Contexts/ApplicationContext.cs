using IdleGameServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdleGameServer.Contexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<User>? User { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Console.WriteLine("create database");
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, Name = "Tom" },
                    new User {Id = 2, Name = "Bob" },
                    new User {Id = 3, Name = "Sam" }
            );
        }
    }
}
