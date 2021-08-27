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
        [Column("plan_id"), ForeignKey(nameof(PlanData))]
        public Guid? PlanId { get; set; }

        public Plan PlanData
        {
            get => LazyLoader.Load(this, ref _planData);
            set => _planData = value;
        }

        [Column("card_final"), MaxLength(20)]
        public string CardFinal { get; set; }
        private User _user;
        private Plan _planData;

        public virtual User User
        {
            get => LazyLoader.Load(this, ref _user);
            set => _user = value;
        }

        
    }
}