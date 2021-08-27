using System;
using System.ComponentModel.DataAnnotations;
using XSpy.Database.Models.Data.Financial;

namespace CFCEad.Shared.Models.Requests.Financial
{
    public class PurchaseDepositRequest
    {
        public Guid PlanId { get; set; }

        public string VoucherCode { get; set; }
    }
}