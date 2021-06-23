using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_files")]
    public class File : BaseDeviceEntity
    {
        [Column("original_name")] public string OriginalName { get; set; }
        [Column("original_path")] public string OriginalPath { get; set; }
        [Column("file_type"), MaxLength(20)] public string FileType { get; set; }
        [Column("file_path")] public string SavedPath { get; set; }

    }
}