using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;
using XSpy.Database.Models.Data.Financial;

namespace XSpy.Database.Entities.Financial
{
    [Table("transactions")]
    public class Transaction : LazyLoaded
    {
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("payment_method")] public PaymentMethod PaymentMethod { get; set; }
        [Column("status")] public PaymentStatus PaymentStatus { get; set; }
        [Column("installments")] public int Installments { get; set; }
        [Column("json_response")] public string JsonResponse { get; set; }
        [Column("extra_data")] public string ExtraData { get; set; }
        [Column("value")] public decimal Value { get; set; }
        [Column("tax_value")] public decimal TaxValue { get; set; }
        [Column("quantity")] public int Quantity { get; set; }
        [Column("created_at")] public DateTime CreatedAt { get; set; }

        [Column("user_id"), ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        private User _user;

        public virtual User User
        {
            get => LazyLoader.Load(this, ref _user);
            set => _user = value;
        }

        [Column("card_id"), ForeignKey(nameof(CardData))]
        public Guid? CardId { get; set; }

        private StoredCreditCard _storedCreditCard;

        public virtual StoredCreditCard CardData
        {
            get => LazyLoader.Load(this, ref _storedCreditCard);
            set => _storedCreditCard = value;
        }
    }
}