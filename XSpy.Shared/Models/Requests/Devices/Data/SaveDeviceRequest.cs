namespace XSpy.Database.Models.Requests.Devices.Data
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