using System.Collections.Generic;

namespace XSpy.Shared.Models.Requests.Devices
{
    public class SaveContactsRequest
    {
        public List<ContactDataRequest> ContactsList { get; set; }
    }

    public class ContactDataRequest
    {
        public string PhoneId { get; set; }
        public string PhoneNo { get; set; }
        public string Name { get; set; }
    }
}