using System.Collections.Generic;

namespace XSpy.Database.Models.Requests.Devices.Data
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