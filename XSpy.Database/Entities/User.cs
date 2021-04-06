using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;
using XSpy.Database.Entities.Roles;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities
{
    [Table("users")]
    public class User : LazyLoaded, IUserEntity
    {
        public User()
        {
        }

        public User(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private Rank _rankData;
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("username"), MaxLength(40)] public string Username { get; set; }
        [Column("password"), MaxLength(100)] public string Password { get; set; }
        [Column("fullname"), MaxLength(140)] public string Name { get; set; }

        [Column("rank_id"), ForeignKey(nameof(Rank))]
        public Guid RankId { get; set; }

        [Column("is_active")] public bool IsActive { get; set; }
        [Column("device_token")] public Guid DeviceToken { get; set; }

        [Column("email"), EmailAddress, MaxLength(120)] public string Email { get; set; }

        public bool HasPermission(string role)
        {
            return RankData.Roles.Any(s => !s.FakeRole && s.RoleName == role || s.RoleName == "IS_ADMIN");
        }
        public Rank RankData
        {
            get => LazyLoader.Load(this, ref _rankData);
            set => _rankData = value;
        }

        IRankEntity IUserEntity.RankData
        {
            get => _rankData;
            set => _rankData = (Rank) value;
        }
    }
}