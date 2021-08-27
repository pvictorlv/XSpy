using System;
using System.ComponentModel.DataAnnotations;
using XSpy.Database.Models.Data.Financial;

namespace CFCEad.Shared.Models.Requests.Financial
{
    public class PurchaseCreditRequest
    {
        public int Amount { get; set; }
        public int Installments { get; set; }
        public string VoucherCode { get; set; }
        [Required] public CreditCard CreditCard { get; set; }
    }
}