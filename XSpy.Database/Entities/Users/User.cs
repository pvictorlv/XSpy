using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;
using XSpy.Database.Entities.Roles;
using XSpy.Database.Interfaces;

namespace XSpy.Database.Entities
{
    [Table("users")]
    public class User : LazyLoaded, IUserEntity
    {
        private Rank _rankData;
        private UserAddress _userAddress;
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("password"), MaxLength(100)] public string Password { get; set; }
        [Column("fullname"), MaxLength(140)] public string Name { get; set; }
        [Column("profile_photo"), MaxLength(300)] public string ProfilePhoto { get; set; }

        [Column("rank_id"), ForeignKey(nameof(Rank))]
        public Guid RankId { get; set; }

        [Column("is_active")] public bool IsActive { get; set; }
        [Column("plan_expire_date")] public DateTime? PlanExpireDate { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("device_token")] public Guid DeviceToken { get; set; }

        [Column("email"), EmailAddress, MaxLength(120)] public string Email { get; set; }
        
        public Rank RankData
        {
            get => LazyLoader.Load(this, ref _rankData);
            set => _rankData = value;
        }

        public UserAddress UserAddress
        {
            get => LazyLoader.Load(this, ref _userAddress);
            set => _userAddress = value;
        }

        IRankEntity IUserEntity.RankData
        {
            get => _rankData;
            set => _rankData = (Rank) value;
        }
    }
}