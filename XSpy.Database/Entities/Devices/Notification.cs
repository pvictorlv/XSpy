using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_notifications")]
    public class Notification : BaseDeviceEntity
    {
        [Column("key")] public string Key { get; set; }
        [Column("content"), MaxLength(500)] public string Content { get; set; }
        [Column("device_date"), MaxLength(30)] public string Date { get; set; }
        [Column("call_type")] public CallType Type { get; set; }

    }
}