using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.XSpy.Shared.Models.Interfaces
{
    public interface IUserEntity
    {
        public Guid Id { get; set; }
        public Guid DeviceToken { get; set; }
        public string Password { get; set; }
        [Column("rank_id")] public Guid RankId { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public IRankEntity RankData { get; }
    }
}