using System;
using System.Collections.Generic;
using XSpy.Database.Entities.Devices;

namespace XSpy.Shared.Models.Views.Device
{
    public class DeviceAppMessagesViewModel
    {
        public Guid DeviceId{ get; set; }
        public List<AppContact> Contacts { get; set; }
    }
}