using System;
using Microsoft.EntityFrameworkCore;
using XSpy.Database.Entities;

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
    }
}