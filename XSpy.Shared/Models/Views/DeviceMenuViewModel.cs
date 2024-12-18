﻿namespace XSpy.Database.Models.Views
{
    public class DeviceMenuViewModel
    {
        public  Database.Entities.Devices.Device Device { get; set; }
        public long Files { get; set; }
        public long WifiCount { get; set; }
        public long Locations { get; set; }
        public long Calls { get; set; }
        public long Instagram { get; set; }
        public long Audios { get; set; }
        public long Words { get; set; }
        public long Messages { get; set; }
        public long Photos { get; set; }
        public long Contacts { get; set; }
        public long WhatsApp { get; set; }
    }
}