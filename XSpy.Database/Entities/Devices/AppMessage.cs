using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_app_messages")]
    public class AppMessage : BaseDeviceEntity
    {
        [Column("contact_name"), MaxLength(255)] public string Address { get; set; }
        [Column("body"), MaxLength(180)] public string Body { get; set; }
        [Column("app_type")] public AppType AppType { get; set; }
    }
}