using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Roles
{
    [Table("ranks")]
    public class Rank : LazyLoaded, IRankEntity
    {
        public Rank()
        {
        }

        public Rank(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        private IEnumerable<RankRole> _roles;
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("name"), MaxLength(50)] public string Name { get; set; }

        public IEnumerable<RankRole> Roles
        {
            get => LazyLoader.Load(this, ref _roles);
            set => _roles = value;
        } 

        IEnumerable<IRankRoleEntity> IRankEntity.Roles
        {
            get => _roles;
            set => _roles = (IEnumerable<RankRole>) value;
        }
    }
}