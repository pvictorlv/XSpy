﻿using System.Collections.Generic;
using XSpy.Database.Entities.Devices;

namespace XSpy.Shared.Models.Views.Device
{
    public class LocationDataViewModel : DeviceDataViewModel
    {
        public Location LatestLocation { get; set; }
        public IEnumerable<Location> Locations { get; set; }
    }
}