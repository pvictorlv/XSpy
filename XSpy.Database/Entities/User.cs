using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Base;
using XSpy.Database.Entities.Roles;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities
{
    [Table("users")]
    public class User : LazyLoaded, IUserEntity
    {
        private Rank _rankData;
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("username")] public string Username { get; set; }
        [Column("password"), MaxLength(100)] public string Password { get; set; }

        [Column("rank_id"), ForeignKey(nameof(Rank))]
        public Guid RankId { get; set; }

        [Column("is_active")] public bool IsActive { get; set; }
        [Column("device_token")] public Guid DeviceToken { get; set; }

        [Column("email"), EmailAddress] public string Email { get; set; }
        public IRankEntity RankData => Rank;

        public Rank Rank
        {
            get => _rankData;
            set => _rankData = (Rank) value;
        }
    }
}