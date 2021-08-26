using System.Collections.Generic;

namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class SaveCallsRequest
    {
        public List<SaveCallData> CallsList { get; set; }
    }

    public class SaveCallData
    {
        public string PhoneNo { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public double Date { get; set; }
        public CallType Type { get; set; }
    }
}