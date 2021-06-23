using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_notifications")]
    public class Notification : BaseDeviceEntity
    {
        [Column("key")] public string Key { get; set; }
        [Column("title")] public string Title { get; set; }
        [Column("app_name")] public string AppName { get; set; }
        [Column("content"), MaxLength(500)] public string Content { get; set; }
        [Column("device_date")] public DateTime Date { get; set; }

    }
}