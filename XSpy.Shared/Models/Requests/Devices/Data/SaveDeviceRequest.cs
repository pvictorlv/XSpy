using System.Runtime.InteropServices;

namespace XSpy.Shared.Models.Requests.Devices
{
    public class SaveDeviceRequest
    {
        public string DeviceId { get; set; }
        public string Release { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string IpAddress { get; set; }
    }
}