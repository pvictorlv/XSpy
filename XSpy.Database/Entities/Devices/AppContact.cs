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
    [Table("device_app_contacts")]
    public class AppContact : BaseDeviceEntity
    {
        [Column("contact_name"), MaxLength(255)] public string ContactName { get; set; }
        [Column("app_type")] public AppType AppType { get; set; }
    }
}