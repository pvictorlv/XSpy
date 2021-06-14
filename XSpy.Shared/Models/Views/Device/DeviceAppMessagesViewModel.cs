using System.Collections.Generic;
using XSpy.Database.Entities.Devices;

namespace XSpy.Shared.Models.Views.Device
{
    public class DeviceAppMessagesViewModel
    {
        public IEnumerable<AppContact> Contacts { get; set; }
    }
}