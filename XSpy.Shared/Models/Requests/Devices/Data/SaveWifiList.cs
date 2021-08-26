namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class SaveWifiList
    {
        public WifiDataList[] Networks { get; set; }
    }

    public class WifiDataList
    {
        public string BSSID { get; set; }
        public string SSID { get; set; }
    }
}