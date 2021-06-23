using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_temp_paths")]
    public class TempPath : BaseDeviceEntity
    {
        [Column("name")] public string Name { get; set; }
        [Column("parent")] public string Parent { get; set; }
        [Column("path")] public string Path { get; set; }
        [Column("can_read")] public bool CanRead { get; set; }
        [Column("is_dir")] public bool IsDir { get; set; }

    }
}