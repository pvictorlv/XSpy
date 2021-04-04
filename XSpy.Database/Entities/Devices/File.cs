using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_files")]
    public class File : BaseDeviceEntity
    {
        [Column("original_name")] public string OriginalName { get; set; }
        [Column("file_path")] public string SavedPath { get; set; }

    }
}