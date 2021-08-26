using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;

namespace XSpy.Database.Entities.Financial
{
    [Table("stored_credit_cards")]
    public class StoredCreditCard : LazyLoaded
    {
        [Key, Column("id")] public Guid Id { get; set; }

        [Column("user_id"), ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        private User _user;

        public virtual User User
        {
            get => LazyLoader.Load(this, ref _user);
            set => _user = value;
        }
    }
}