using XSpy.Database.Models.Data.Financial;

namespace CFCEad.Shared.Models.Views.Financial
{
    public class InvoiceDataViewModel
    {
        public TransactionData Transaction { get; set; }

        public decimal VoucherDiscount { get; set; }
    }
}