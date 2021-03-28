using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Roles
{
    [Table("rank_roles")]
    public class RankRole : LazyLoaded, IRankRoleEntity
    {
        public RankRole()
        {
            
        }

        public RankRole(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private Roles _roleData;
        [Key] public Guid Id { get; set; }

        [Column("rank_id"), ForeignKey(nameof(Rank))]
        public Guid RankId { get; set; }

        [Column("fake_role")] public bool FakeRole { get; set; }

        [ForeignKey(nameof(RoleData)), Column("role_name")]
        public string RoleName { get; set; }

        public Roles RoleData
        {
            get => LazyLoader.Load(this, ref _roleData);
            set => _roleData = value;
        }

        public Rank Rank { get; set; }
    }
}