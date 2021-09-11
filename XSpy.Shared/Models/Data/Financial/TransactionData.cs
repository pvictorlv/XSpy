using System;
using Stock.Shared.Models.Data;
using XSpy.Database.Models.Data.Financial.Product;

namespace XSpy.Database.Models.Data.Financial
{
    public class TransactionData
    {
        public Guid? Id { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string ExtraData { get; set; }
        public decimal Value { get; set; }
        public PlanData PlanData { get; set; }
        public UserData UserData { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}