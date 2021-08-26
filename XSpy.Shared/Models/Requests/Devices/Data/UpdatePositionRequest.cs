namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class UpdatePositionRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Accuracy { get; set; }
        public double Speed { get; set; }
        public bool Enabled { get; set; }
    }
}