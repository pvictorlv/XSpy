using System.Collections.Generic;

namespace XSpy.Shared.Models.Requests.Devices
{
    public class SaveNotificationsData
    {
        public string AppName { get; set; }
        public string Title { get; set; }
        public long PostTime { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
    }
}