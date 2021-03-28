using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;
using XSpy.Database.Entities.Roles;

namespace XSpy.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(s => s.RankData);

            AddDataSeed(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void AddDataSeed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@admin.com",
                IsActive = true,
                Password = "changeme",
                DeviceToken = Guid.NewGuid(),
                RankId = Ranks.FirstOrDefault(s => s.Name == "Admin").Id
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Device> Messages { get; set; }
    }
}