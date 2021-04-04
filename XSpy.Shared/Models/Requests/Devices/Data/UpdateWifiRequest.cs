namespace XSpy.Shared.Models.Requests.Devices
{
    public class UpdateWifiRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public double Speed { get; set; }
        public double Enabled { get; set; }
    }
}