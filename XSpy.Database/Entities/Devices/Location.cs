﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_locations")]
    public class Location : BaseDeviceEntity
    {
        [Column("latitude")] public double Latitude { get; set; }
        [Column("longitude")] public double Longitude { get; set; }
        [Column("altitude")] public double Altitude { get; set; }
        [Column("accuracy")] public double Accuracy { get; set; }
        [Column("speed")] public double Speed { get; set; }
        [Column("is_enabled")] public bool Enabled { get; set; }

    }
}