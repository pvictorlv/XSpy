using System;
using Microsoft.EntityFrameworkCore;
using XSpy.Database.Entities;
using XSpy.Database.Entities.Devices;

namespace XSpy.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Device> Messages { get; set; }
    }
}