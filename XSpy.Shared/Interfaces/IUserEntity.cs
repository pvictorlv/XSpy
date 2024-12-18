﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.Interfaces
{
    public interface IUserEntity
    {
        public Guid Id { get; set; }
        public Guid DeviceToken { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        [Column("rank_id")] public Guid RankId { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public IRankEntity RankData { get; set; }
    }
}