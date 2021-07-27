using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CFCEad.Shared.Models.Financial
{
    [Table("transactions")]
    public class Transaction : LazyLoaded
    {
        private Voucher _voucher;
        private User _user;
        [Key, Column("id")] public Guid Id { get; set; }
        [Column("payment_type")] public PaymentType PaymentType { get; set; }
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

        [Column("voucher_id"), ForeignKey(nameof(Voucher))]
        public Guid? VoucherId { get; set; }

        public virtual User User
        {
            get => LazyLoader.Load(this, ref _user);
            set => _user = value;
        }

        public virtual Voucher Voucher
        {
            get => LazyLoader.Load(this, ref _voucher);
            set => _voucher = value;
        }

        public Transaction()
        {
            
        }

        public Transaction(ILazyLoader lazyLoader) : base(lazyLoader)
        {
            
        }
    }
}