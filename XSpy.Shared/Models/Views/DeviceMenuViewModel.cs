using System.Collections.Generic;

namespace XSpy.Shared.Models.Views
{
    public class DeviceMenuViewModel
    {
        public long Files { get; set; }
        public long WifiCount { get; set; }
        public long Locations { get; set; }
        public long Calls { get; set; }
        public int BatteryLevel { get; set; }
        public long Audios { get; set; }
        public long Words { get; set; }
        public long Messages { get; set; }
        public long Photos { get; set; }
        public long Contacts { get; set; }
        public long WhatsApp { get; set; }
    }
}