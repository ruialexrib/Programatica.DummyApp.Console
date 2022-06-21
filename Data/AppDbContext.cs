using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Programatica.DummyApp.Console.Models;
using Programatica.Framework.Data.Context;
using System.Reflection;

namespace Programatica.DummyApp.Console.Data
{
    public class AppDbContext : BaseDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // seed data
            modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, Name = "User1", Email = "user1@mail.com", Password = "123" },
                    new User { Id = 2, Name = "User2", Email = "user2@mail.com", Password = "123" }
            );
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContextFactory() { }

        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .AddUserSecrets(Assembly.GetExecutingAssembly())
               .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseInMemoryDatabase(connectionString);
            return new AppDbContext(builder.Options);
        }
    }
}
