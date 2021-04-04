namespace XSpy.Shared.Models.Requests.Devices
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