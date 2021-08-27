
using XSpy.Database.Models.Data.Financial;

namespace CFCEad.Shared.Models.Requests.Financial
{
    public class ListOrdersRequest
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}